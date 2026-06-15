using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetRolesAsync(List<string> roleIds, CancellationToken cancellationToken);
    }
}
