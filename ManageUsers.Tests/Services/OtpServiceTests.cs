using FluentAssertions;
using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Implementations;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using ManageUsers.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace ManageUsers.Tests.Services;

public class OtpServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;
    private readonly Application.Interfaces.ISmsService _smsService;

    public OtpServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _smsService = Substitute.For<Application.Interfaces.ISmsService>();
        _userService = Substitute.For<Application.Services.Interfaces.IUserService>();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@example.com",
            PhoneNumber = "09123456789",
            FirstName = "Test",
            LastName = "User",
            NationalCode = "1234567890",
            Enabled = true
        };
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GenerateOtpAsync_CreatesOtpRecordInDatabase()
    {
        var code = await _userService.GenerateOtpAsync("09123456789");

        code.Should().HaveLength(6);
        code.Should().MatchRegex("^[0-9]{6}$");

        var record = await _context.Users.FirstOrDefaultAsync(o => o.Id == 1);
        record.Should().NotBeNull();
        record!.OTPCode.Should().Be(SecurityHelper.GetSha256Hash(code));
        record!.SendDateTimeOTPCode.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateOtpAsync_GeneratesDifferentCodesOnEachCall()
    {
        var code1 = await _userService.GenerateOtpAsync("09123456789");
        var code2 = await _userService.GenerateOtpAsync("09123456789");

        code1.Should().NotBe(code2);

        var records = await _context.Users.Where(o => o.Id == 1).ToListAsync();
        records.Should().HaveCount(2);
    }
}
