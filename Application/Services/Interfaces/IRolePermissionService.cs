using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IRolePermissionService
    {
        Task<Dictionary<string, List<string>>> GetRolePermissionsByUserRolesAsync(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default);

        Task<List<RolePermission>> GetRolePermisionsByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken = default);
        Task<List<RolePermission>> GetRolePermisionsByRoleIdAsync(string roleId, CancellationToken cancellationToken = default);

    }
}
