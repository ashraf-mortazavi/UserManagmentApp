using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<List<IdentityUserRole<int>>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    }
}
