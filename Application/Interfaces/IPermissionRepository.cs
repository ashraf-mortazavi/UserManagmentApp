using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetPermisionsByPermissionIds(List<string> permissionIds, CancellationToken cancellationToken = default);
    }
}
