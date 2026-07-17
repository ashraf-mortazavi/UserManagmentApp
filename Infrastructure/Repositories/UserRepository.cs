using ManageUsers.Application.Common;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageUsers.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersWithFilterAsync(string filter, int page, int pagesize, AccessLevel accessLevel, int? areaId, int? zoneId, int roleId, CancellationToken cancellationToken)
        {
            var visibleRoleIds = await GetVisibleRoleIdsAsync(roleId);

            IQueryable<User> query = _context.Users
                .Include(u => u.Area)
                .Include(u => u.Zone)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

            query = ApplyRoleFilter(query, visibleRoleIds);
            query = ApplyAccessLevelFilter(query, accessLevel, areaId, zoneId);

            if (!string.IsNullOrEmpty(filter))
            {
                string toUpperFilter = filter.ToUpper();
                query = query.Where(u => u.FirstName.ToUpper().Contains(toUpperFilter) ||
                    u.LastName.ToUpper().Contains(toUpperFilter) ||
                    u.NationalCode.ToUpper().Contains(toUpperFilter) ||
                    u.Enabled.ToString() == toUpperFilter ||
                    u.UserRoles.Select(r => r.Role.Name.ToUpper().Contains(toUpperFilter)).FirstOrDefault());
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(string filter, AccessLevel accessLevel, int? areaId, int? zoneId, int roleId, CancellationToken cancellationToken)
        {
            var visibleRoleIds = await GetVisibleRoleIdsAsync(roleId);

            IQueryable<User> query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

            query = ApplyRoleFilter(query, visibleRoleIds);
            query = ApplyAccessLevelFilter(query, accessLevel, areaId, zoneId);


            if (!string.IsNullOrEmpty(filter))
            {
                string toUpperFilter = filter.ToUpper();
                query = query.Where(u => u.FirstName.ToUpper().Contains(toUpperFilter) ||
                    u.LastName.ToUpper().Contains(toUpperFilter) ||
                    u.NationalCode.ToUpper().Contains(toUpperFilter) ||
                    u.Enabled.ToString() == toUpperFilter || 
                    u.UserRoles.Select(r => r.Role.Name.ToUpper().Contains(toUpperFilter)).FirstOrDefault());
            }

            return await query.CountAsync(cancellationToken);
        }

        private async Task<HashSet<int>> GetVisibleRoleIdsAsync(int roleId)
        {
            var visibleRoleIds = new HashSet<int>();
            await TraverseRoleHierarchyAsync(roleId, visibleRoleIds);
            return visibleRoleIds;
        }

        private IQueryable<User> ApplyAccessLevelFilter(IQueryable<User> query, AccessLevel accessLevel, int? areaId, int? zoneId)
        {
            return accessLevel switch
            {
                AccessLevel.Setad => query,
                AccessLevel.Area => query.Where(u =>
                    (u.AreaId == areaId) ||
                    (u.Zone != null && u.Zone.Id == zoneId)),
                AccessLevel.Zone => query.Where(u =>
                    u.AreaId == areaId && u.ZonId == zoneId),
                _ => query
            };
        }

        private IQueryable<User> ApplyRoleFilter(IQueryable<User> query, HashSet<int> visibleRoleIds)
        {
            return query.Where(u => u.UserRoles.Any(ur => visibleRoleIds.Contains(ur.RoleId)));
        }

     

        private async Task TraverseRoleHierarchyAsync(int roleId, HashSet<int> visibleRoleIds)
        {
            if (!visibleRoleIds.Add(roleId))
                return;

            var role = await _context.Roles
                .Where(r => r.Id == roleId)
                .Select(r => new { r.NextLowerRoleId })
                .FirstOrDefaultAsync();

            if (role?.NextLowerRoleId.HasValue == true)
            {
                await TraverseRoleHierarchyAsync(role.NextLowerRoleId.Value, visibleRoleIds);
            }
        }


        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _context.Users
             .Include(u => u.UserRoles)
             .ThenInclude(ur => ur.Role)
             .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId, ct);
        }

        public async Task<User?> GetUserByIdWithRolesAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.Area)
                .Include(u => u.Zone)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId, ct);
        }

        public async Task<string> GetUserRoleIdsAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId.ToString())
                .FirstOrDefaultAsync(ct);
        }

        public async Task<User?> GetUserByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.NationalCode == nationalCode, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Update(user);
        }
    }
}
