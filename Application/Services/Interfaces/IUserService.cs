using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);
        Task<User?> GetUserByIdWithRolesAsync(string userId, CancellationToken ct = default);
        Task<List<string>> GetUserRoleIdsAsync(int userId, CancellationToken ct = default);
        Task<IdentityResult> UpdateUserRolesAsync(User user, List<string> newRoleNames, CancellationToken ct = default);
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<User?> GetUserByPhoneNumber(string phoneNumber, CancellationToken ct = default);
        Task<User?> GetUserByNationalCodeAsync(string nationalCode, CancellationToken ct = default);
        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword);

        Task<IdentityResult> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default);

        Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default);
        
        void UpdateUser(User user, CancellationToken cancellationToken = default);

        Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<bool> ValidateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<bool> IsLockedOutAsync(User user);
        Task RegisterFailedAttemptAsync(User user);
        Task ResetFailedAttemptsAsync(User user);
        Task<int> GetAccessFailedCountAsync(User user);

        Task<List<User>> GetAllUsersAsync(string? searchTerm, int pageNumber, int pageSize, AccessLevel callerAccessLevel, int? callerAreaId, int? callerRegionId, CancellationToken ct);

        Task<int> GetTotalCountAsync(string? searchTerm, AccessLevel callerAccessLevel, int? callerAreaId, int? callerRegionId, CancellationToken ct);

        Task<List<Area>> GetAllAreasAsync(CancellationToken ct = default);
        Task<List<Region>> GetRegionsByAreaAsync(int areaId, CancellationToken ct = default);

    }
}
