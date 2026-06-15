using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<string> roleIds, CancellationToken cancellationToken = default);
    }
}
