using Azure.Core;
using ManageUsers.Application.Common;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManageUsers.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _usermanager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JWTSetting _jwtSettings;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(AppDbContext context, UserManager<User> usermanager, RoleManager<Role> roleManager,
            SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor,
            IOptions<JWTSetting> jwtOptions)
        {
            _context = context;
            _usermanager = usermanager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId, ct);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName, ct);
        }

        public async Task<User> AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
            return user;
        }

        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string userName, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName, ct);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _usermanager.CheckPasswordAsync(user: user, password: password);
        }

        public async Task<List<IdentityUserRole<string>>> GetUserRole(string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
                throw new ArgumentException("Invalid user ID format", nameof(userId));

            return await _context.UserRoles.Where(ur => ur.UserId == userGuid)
                 .Select(ur => new IdentityUserRole<string>
                 {
                     UserId = userId,
                     RoleId = ur.RoleId.ToString()
                 }).ToListAsync();
        }

        public async Task<List<Role>> GetRolesAsync(List<string> roles, CancellationToken cancellationToken = default)
        {
            if (roles is null || roles.Count == 0)
            {
                return new List<Role>();
            }
            List<string> normalizedRoleNames = roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpper()).ToList();
            return await _roleManager.Roles.Where(r => normalizedRoleNames.Contains(r.Name!.ToUpper())).ToListAsync(cancellationToken);
        }

        //public async Task<Dictionary<Guid, List<Guid>>> GetRoleIdsPermissionIds(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default)
        //{
        //    List<RolePermission> rolePermissions = [];

        //    List<string> roleIds = identityUserRoles
        //        .Select(r => r.RoleId)
        //        .Distinct()
        //        .ToList();


        //    var permisionRoloPaires = await _context.RolePermissions
        //         .Where(rp => roleIds.Contains(rp.RoleId.ToString()))
        //         .Select(rp => new { rp.PermissionId, rp.RoleId })
        //         .ToListAsync(cancellationToken);


        //    var permissionRolesMap = permisionRoloPaires
        //        .GroupBy(x => x.PermissionId)
        //        .ToDictionary(
        //            g => g.Key,                       // PermissionId
        //            g => g.Select(x => x.RoleId)      // Roles that have this permission
        //                  .Distinct()
        //                  .ToList()
        //        );

        //    return permissionRolesMap;
        //}

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

            await SaveToken(userToken, context.User, cancellationToken);

            return new JWEToken
            {
                Token = accessToken,
                RefreshToken = refreshToken.ToString()
            };
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

        private async Task SaveToken(ApplicationUserToken userToken, User? user, CancellationToken cancellationToken = default)
        {
            if (user is not null) // Check Duplicate Token
            {
                var existingTokens = await _context.ApplicationUserTokens.Where(u => u.UserId == user.Id).ToListAsync();

                if (existingTokens.Any())
                {
                    _context.ApplicationUserTokens.RemoveRange(existingTokens);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await _context.ApplicationUserTokens.AddAsync(userToken, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Dictionary<string, List<string>>> GetRolePermissions(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default)
        {
            List<RolePermission> rolePermissions = [];

            List<string> roleIds = identityUserRoles
                .Select(r => r.RoleId)
                .Distinct()
                .ToList();


            var permisionRoloPaires = await _context.RolePermissions
                 .Where(rp => roleIds.Contains(rp.RoleId.ToString()))
                 .Select(rp => new { rp.PermissionId, rp.RoleId })
                 .Distinct()
                 .ToListAsync(cancellationToken);


            var permissionRolesMap = permisionRoloPaires
                .GroupBy(x => x.RoleId.ToString())
                .ToDictionary(
                    g => g.Key,                      
                    g => g.Select(x => x.PermissionId.ToString())      
                          .Distinct()
                          .ToList()
                );

            return permissionRolesMap;
        }

        public async Task<User> AssignUserRolesAsync(User user, string password,List<string> roles, CancellationToken cancellationToken = default)
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

        public async Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<string> roleIds, CancellationToken cancellationToken = default)
        {
            if (roleIds is null || roleIds.Count == 0)
            {
                return new List<RolePermission>();
            }


            List<string> existingRoleIds = await _context.Roles
                .Where(r => roleIds.Contains(r.Id.ToString()))
                .Select(r => r.Id.ToString())
                .ToListAsync(cancellationToken);

            if (!existingRoleIds.Any())
            {
                return new List<RolePermission>();
            }

            List<RolePermission> rolePermissions = await _context.RolePermissions
                .Where(rp => existingRoleIds.Contains(rp.RoleId.ToString()))
                .Distinct()
                .ToListAsync(cancellationToken);

            return rolePermissions;
        }

    }
}