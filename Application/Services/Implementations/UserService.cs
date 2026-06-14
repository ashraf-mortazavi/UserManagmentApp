using ManageUsers.Application.Common;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ManageUsers.Application.Services.Implementations;

public class UserService(IUnitOfWork unitOfWork, UserManager<User> userManager) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<User> _usermanager = userManager;
    private readonly JWTSetting _jwtSettings;
    private readonly SignInManager<User> _signInManager;

    public async Task<User> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default)
    {
        try
        {
            var createResult = await _usermanager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                if (createResult.Errors.Any(e => e.Code == "DuplicateUserName"))
                    throw new InvalidOperationException("A user with that name already exists.");

                throw new Exception($"User creation failed: {string.Join(", ", createResult.Errors)}");
            }

            var addRoleResult = await _usermanager.AddToRolesAsync(user, roles);
            if (!addRoleResult.Succeeded)
                throw new Exception("Failed to assign roles.");
        }
        catch (Exception ex)
        {
            throw;
        }

        return user;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _usermanager.CheckPasswordAsync(user: user, password: password);
    }

    public async Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        double expirationHours = _jwtSettings.TokenExpirationInHours;
        int refreshDays = _jwtSettings.RefreshTokenExpirationInDays;

        // 1. Claims
        var claims = await GetClaimsAsync(
            context.User,
            context.ActiveRole,
            context.RolePermissions,
            cancellationToken);

        // 2. Signing key
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);


        // 4. Token descriptor – no NotBefore unless you have a real use‑case
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddHours(expirationHours),
            SigningCredentials = signingCredentials,
            IssuedAt = now
        };

        // 5. Generate encrypted access token
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        string accessToken = handler.CreateEncodedJwt(descriptor);

        // 6. Refresh token
        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);
        Guid refreshToken = new(randomBytes);

        var userToken = new ApplicationUserToken
        {
            TokenExpirationDate = now.AddHours(expirationHours),
            HashToken = SecurityHelper.GetSha256Hash(accessToken),  // optional, keep if you need revocation
            UserId = context.User.Id,
            RefreshToken = SecurityHelper.GetSha256Hash(refreshToken.ToString()),
            RefreshTokenExpirationDate = now.AddDays(refreshDays),
            CreatedAt = now
        };

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

    public async Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword)
    {
        User? user = await _usermanager.FindByIdAsync(userId);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        if (await _usermanager.HasPasswordAsync(user))
        {
            var remove = await _usermanager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
                return remove;
        }

        return await _usermanager.AddPasswordAsync(user, newPassword);
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
}
