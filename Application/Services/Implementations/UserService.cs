using ManageUsers.Application.Common;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ManageUsers.Application.Services.Implementations;

public class UserService(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JWTSetting> jwtSettings) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly JWTSetting _jwtSettings = jwtSettings.Value;

    private const int MinimumRangeOTP = 0;
    private const int MaximumRangeOTP = 100000;

    public async Task<IdentityResult> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default)
    {
        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            if (createResult.Errors.Any(e => e.Code == "DuplicateUserName"))
                return IdentityResult.Failed(new IdentityError { Description = "A user with that name already exists." });

            if (createResult.Errors.Any(e => e.Code == "DuplicateNationalCode"))
                return IdentityResult.Failed(new IdentityError { Description = "A user with that national code already exists." });
            
            return IdentityResult.Failed(new IdentityError { Description = $"User creation failed: {string.Join(", ", createResult.Errors)}" });
        }
        var addRoleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!addRoleResult.Succeeded)
            return IdentityResult.Failed(new IdentityError { Description = "Failed to assign roles." });

        return createResult;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user: user, password: password);
    }

    public async Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        double expirationHours = _jwtSettings.TokenExpirationInHours;
        int refreshDays = _jwtSettings.RefreshTokenExpirationInDays;

        var claims = await GetClaimsAsync(
            context.User,
            context.ActiveRole,
            context.RolePermissions,
            cancellationToken);

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddHours(expirationHours),
            SigningCredentials = signingCredentials,
            IssuedAt = now
        };

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        string accessToken = handler.CreateEncodedJwt(descriptor);

        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);
        Guid refreshToken = new(randomBytes);

        var userToken = new ApplicationUserToken
        {
            TokenExpirationDate = now.AddHours(expirationHours),
            HashToken = SecurityHelper.GetSha256Hash(accessToken),
            UserId = context.User.Id,
            RefreshToken = SecurityHelper.GetSha256Hash(refreshToken.ToString()),
            RefreshTokenExpirationDate = now.AddDays(refreshDays),
            CreatedAt = now
        };

        await _unitOfWork.SaveChangesAsync();

        return new JWEToken
        {
            Token = accessToken,
            RefreshToken = refreshToken.ToString()
        };
    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetByUserNameAsync(userName, cancellationToken: cancellationToken);
    }

    public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        return await _unitOfWork.Users.GetUserByIdAsync(userId, ct: ct);
    }

    public async Task<User?> GetUserByPhoneNumber(string phoneNumber, CancellationToken ct = default)
    {
        return await _unitOfWork.Users.GetByPhoneNumberAsync(phoneNumber, cancellationToken: ct);
    }

    public async Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        if (await _userManager.HasPasswordAsync(user))
        {
            var remove = await _userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
                return remove;
        }

        return await _userManager.AddPasswordAsync(user, newPassword);
    }

    public async Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.Now;
        User user = await _unitOfWork.Users.GetByPhoneNumberAsync(phoneNumber, cancellationToken: cancellationToken);
        if (user is not null)
        {
            user.OTPCode = GenerateSecureCode();
            user.SendDateTimeOTPCode = now;
        }
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return user.OTPCode;
    }

    public Task<bool> ValidateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(User user, bool activeRule,
          List<RolePermission>? rolePermissions = null,
            CancellationToken cancellationToken = default)
    {
        var result = await _signInManager.ClaimsFactory.CreateAsync(user);
        List<Claim> list = [];

        var claims = new List<Claim>(result.Claims)
            {
                new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber),
            };

        return claims;
    }

    private static string GenerateSecureCode()
    {
        return new Random().Next(MinimumRangeOTP, MaximumRangeOTP).ToString("D5");
    }

    public Task<bool> IsLockedOutAsync(User user)
    {
        return _userManager.IsLockedOutAsync(user);
    }

    public Task RegisterFailedAttemptAsync(User user)
    {
        return _userManager.AccessFailedAsync(user);
    }

    public Task ResetFailedAttemptsAsync(User user)
    {
        return _userManager.ResetAccessFailedCountAsync(user);
    }

    public Task<int> GetAccessFailedCountAsync(User user)
    {
        return _userManager.GetAccessFailedCountAsync(user);
    }

    public async Task<User?> GetUserByNationalCodeAsync(string nationalCode, CancellationToken ct = default)
    {
        return await _unitOfWork.Users.GetUserByNationalCodeAsync(nationalCode, cancellationToken: ct);

    }

    public void UpdateUser(User user, CancellationToken cancellationToken = default)
    {
       _unitOfWork.Users.Update(user);
    }

    public async Task<List<User>> GetAllUsersAsync(string? searchTerm, int pageNumber, int pageSize, CancellationToken ct)
    {
        List<User> users = new();
        users = await _unitOfWork.Users.GetAllUsersWithFilterAsync(searchTerm,pageNumber, pageSize, ct);
        return users;
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm, CancellationToken ct)
    {
        List<User> users = new();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            users = await _unitOfWork.Users.GetAllUsersWithFilterAsync(searchTerm, 0, 0, ct);
        }
        return users.Count();
    }
}
