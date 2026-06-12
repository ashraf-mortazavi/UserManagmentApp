using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<IdentityUserRole<int>>> GetUserRole(int userId);
        //Task<Dictionary<Guid, List<Guid>>> GetRoleIdsPermissionIds(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default);
        Task<Dictionary<int, List<int>>> GetRolePermissions(IEnumerable<IdentityUserRole<int>> identityUserRoles, CancellationToken cancellationToken = default);
        
        Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<int> roleIds, CancellationToken cancellationToken = default);

        Task<Organization> OrganizationExistsAsync(int organizationId, CancellationToken cancellationToken = default);


    }
}
