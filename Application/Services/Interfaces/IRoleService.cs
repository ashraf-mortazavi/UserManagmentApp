using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default);
        Task<List<Role?>> GetRolesAsync(List<string> roleIds, CancellationToken cancellationToken = default);
    }
}
