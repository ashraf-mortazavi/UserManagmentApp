using Azure.Core;
using ManageUsers.Application;
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
using System.Threading;

namespace ManageUsers.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
      
        public UserRepository(AppDbContext context, UserManager<User> usermanager, RoleManager<Role> roleManager,
            SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor,
            IOptions<JWTSetting> jwtOptions)
        {
            _context = context;
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _context.Users
             .Include(u => u.UserRoles)
             .ThenInclude(ur => ur.Role)
             .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId, ct);

        }
    }
}