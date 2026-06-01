using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<User> AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<List<IdentityUserRole<string>>> GetUserRole(string userId);
        Task<List<Role?>> GetRolesAsync(List<string> roles, CancellationToken cancellationToken = default);
        //Task<Dictionary<Guid, List<Guid>>> GetRoleIdsPermissionIds(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default);
        Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default);
        Task<Dictionary<string, List<string>>> GetRolePermissions(IEnumerable<IdentityUserRole<string>> identityUserRoles, CancellationToken cancellationToken = default);
        Task<User> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default);
        Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword);
        Task<List<RolePermission>> GetRolePermisionsByRoleIds(List<string> roleIds, CancellationToken cancellationToken = default);

    }
}
