using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword);

        Task<User> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default);

        Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default);

    }
}
