using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IRolePermissionService
    {
        Task<Dictionary<int, List<int>>> GetRolePermissionsByUserRolesAsync(IEnumerable<IdentityUserRole<int>> identityUserRoles, CancellationToken cancellationToken = default);

        Task<List<RolePermission>> GetRolePermisionsByRoleIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default);
    }
}
