using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class RolePermissionRepository(AppDbContext appDbContext) : IRolePermissionRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        public async Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<string> roleIds, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.RolePermissions
                  .Where(rp => roleIds.Contains(rp.RoleId.ToString()))
                  .ToListAsync(cancellationToken);
        }

        public Task<Dictionary<int, List<int>>> GetRolePermissions(IEnumerable<IdentityUserRole<int>> identityUserRoles, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
