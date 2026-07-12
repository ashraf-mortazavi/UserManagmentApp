using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetRolePermisionsByIdsAsync(List<string> permissionIds, CancellationToken cancellationToken = default);
    }
}
