using FluentAssertions;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Handlers;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace ManageUsers.Tests.Handlers;

public class LoginUserCommandHandlerTests
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userRoleService = Substitute.For<IUserRoleService>();
        _rolePermissionService = Substitute.For<IRolePermissionService>();
        _handler = new LoginUserCommandHandler(_userService, _userRoleService, _rolePermissionService);
    }

    private User CreateTestUser(int id = 1, bool enabled = true)
    {
        return new User
        {
            Id = id,
            UserName = "testuser",
            Email = "test@example.com",
            PhoneNumber = "09123456789",
            FirstName = "Test",
            LastName = "User",
            NationalCode = "1234567890",
            Enabled = enabled
        };
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsOtpRequiredResponse()
    {
        var user = CreateTestUser();
        _userService.GetByUserNameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);
        _userService.CheckPasswordAsync(user, "Correct@123")
            .Returns(true);
     

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "testuser", Password = "Correct@123" });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.RequiresOtp.Should().BeTrue();
        result.UserId.Should().Be("1");
        result.Token.Should().BeNull();
        result.RefreshToken.Should().BeNull();
    }

    [Fact]
    public async Task Handle_InvalidUsername_ThrowsUserNotFoundException()
    {
        _userService.GetByUserNameAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "nonexistent", Password = "password" });

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DisabledUser_ThrowsUserDisabledException()
    {
        var user = CreateTestUser(enabled: false);
        _userService.GetByUserNameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "testuser", Password = "password" });

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsInvalidCredentialsException()
    {
        var user = CreateTestUser();
        _userService.GetByUserNameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);
        _userService.CheckPasswordAsync(user, "WrongPassword")
            .Returns(false);

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "testuser", Password = "WrongPassword" });

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCredentials_GeneratesAndSendsOtp()
    {
        var user = CreateTestUser();
        _userService.GetByUserNameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);
        _userService.CheckPasswordAsync(user, "Correct@123")
            .Returns(true);

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "testuser", Password = "Correct@123" });

        await _handler.Handle(command, CancellationToken.None);

    }

    [Fact]
    public async Task Handle_UserWithNullEmail_DoesNotThrowWhenSendingOtp()
    {
        var user = CreateTestUser();
        user.Email = null;
        _userService.GetByUserNameAsync("testuser", Arg.Any<CancellationToken>())
            .Returns(user);
        _userService.CheckPasswordAsync(user, "Correct@123")
            .Returns(true);

        var command = new LoginUserCommand(null!, new LoginUserRequest { UserName = "testuser", Password = "Correct@123" });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.RequiresOtp.Should().BeTrue();
    }
}
