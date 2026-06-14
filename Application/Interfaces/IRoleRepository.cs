using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<string>> GetRolesAsync(List<int> roleIds, CancellationToken cancellationToken);
    }
}
