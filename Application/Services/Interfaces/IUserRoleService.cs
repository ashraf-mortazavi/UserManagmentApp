using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<IdentityUserRole<string>>> GetUserRole(int userId, CancellationToken cancellationToken =default);
    }
}
