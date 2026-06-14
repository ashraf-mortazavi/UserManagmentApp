using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<Dictionary<int, List<int>>> GetRolePermissions(IEnumerable<IdentityUserRole<int>> identityUserRoles, CancellationToken cancellationToken = default);

        Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<string> roleIds, CancellationToken cancellationToken = default);
    }
}
