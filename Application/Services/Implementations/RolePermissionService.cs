using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Application.Services.Implementations
{
    public class RolePermissionService(IUnitOfWork unitOfWork) : IRolePermissionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<RolePermission>> GetRolePermisionsByRoleIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default)
        {
            if (roleIds is null || roleIds.Count == 0)
            {
                return new List<RolePermission>();
            }

            List<string> existingRoleIds = await _unitOfWork.Roles.GetRolesAsync(roleIds: roleIds, cancellationToken: cancellationToken);

            if (!existingRoleIds.Any())
            {
                return new List<RolePermission>();
            }

            List<RolePermission> rolePermissions = await _unitOfWork.RolePermissions
                .GetRolePermisionsByRoleIds(existingRoleIds, cancellationToken);

            return rolePermissions;
        }

        public async Task<Dictionary<int, List<int>>> GetRolePermissionsByUserRolesAsync(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default)
        {
            List<RolePermission> rolePermissions = [];

            List<string> roleIds = identityUserRoles
                .Select(r => r.RoleId)
                .Distinct()
                .ToList();


            var permisionRoloPaires = await _unitOfWork.RolePermissions
                 .GetRolePermisionsByRoleIds(roleIds, cancellationToken);

            var x = permisionRoloPaires
                .Select(rp => new { rp.PermissionId, rp.RoleId })
                .Distinct()
                .ToList();



            var permissionRolesMap = permisionRoloPaires
                .GroupBy(x => x.RoleId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.PermissionId)
                          .Distinct()
                          .ToList()
                );

            return permissionRolesMap;
        }
    }
}
