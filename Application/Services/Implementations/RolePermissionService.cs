using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Application.Services.Implementations
{
    public class RolePermissionService(IUnitOfWork unitOfWork) : IRolePermissionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<RolePermission>> GetRolePermisionsByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken = default)
        {
            if (roleIds is null || roleIds.Count == 0)
            {
                return new List<RolePermission>();
            }

            List<Role> existingRoles = await _unitOfWork.Roles.GetRolesAsync(roleIds: roleIds, cancellationToken: cancellationToken);

            if (!existingRoles.Any())
            {
                return new List<RolePermission>();
            }

            List<RolePermission> rolePermissions = await _unitOfWork.RolePermissions
                .GetRolePermisionsByRoleIds(existingRoles.Select(e => e.Id.ToString()).ToList(), cancellationToken);

            return rolePermissions;
        }

        public async Task<Dictionary<string, List<string>>> GetRolePermissionsByUserRolesAsync(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default)
        {
            List<RolePermission> rolePermissions = [];

            List<string> roleIds = identityUserRoles
                .Select(r => r.RoleId)
                .Distinct()
                .ToList();


             rolePermissions = await _unitOfWork.RolePermissions
                 .GetRolePermisionsByRoleIds(roleIds, cancellationToken);

            var permisionRoloPaires = rolePermissions
                .Select(rp => new { rp.PermissionId, rp.RoleId })
                .Distinct()
                .ToList();



            var permissionRolesMap = permisionRoloPaires
                .GroupBy(x => x.RoleId)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Select(x => x.PermissionId.ToString())
                          .Distinct()
                          .ToList()
                );

            return permissionRolesMap;
        }
    }
}
