using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);
        Task<User?> GetUserByIdWithRolesAsync(string userId, CancellationToken ct = default);
        Task<string> GetUserRoleIdsAsync(int userId, CancellationToken ct = default);
        Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<User?> GetUserByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<List<User>> GetAllUsersWithFilterAsync(string filter, int page, int pagesize, AccessLevel accessLevel, int? areaId, int? zoneId, int roleId, CancellationToken cancellationToken);
        Task<int> GetTotalCountAsync(string filter, AccessLevel accessLevel, int? areaId, int? zoneId, int roleId, CancellationToken cancellationToken);
        void Update(User user);
    }
}
