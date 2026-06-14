using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<IdentityUserRole<int>>> GetUserRole(int userId, CancellationToken cancellationToken =default);
    }
}
