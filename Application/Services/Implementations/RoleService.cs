using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Application.Services.Implementations
{
    public class RoleService(IUnitOfWork unitOfWork, UserManager<User> userManager,
    RoleManager<Role> roleManager) : IRoleService
    {
        private readonly UserManager<User> _usermanager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;
        public async Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default)
        {
            return await _roleManager.Roles.ToListAsync(cancellationToken: ct);
        }

        public async Task<Role?> GetRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles.Where(r => r.Id.ToString() == roleId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Role?>> GetRolesAsync(List<string> roleIds, CancellationToken cancellationToken = default)
        {
            if (roleIds is null || roleIds.Count == 0 || !roleIds.Any())
            {
                return new List<Role>();
            }
            return await _roleManager.Roles.Where(r => roleIds.Contains(r.Id.ToString())!).ToListAsync(cancellationToken);
        }
    }
}
