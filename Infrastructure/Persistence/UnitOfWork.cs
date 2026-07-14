using ManageUsers.Application.Common;
using ManageUsers.Application.Interfaces;

namespace ManageUsers.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(
            AppDbContext context,
            IUserRepository users,
            IRoleRepository roles,
            IUserRoleRepository userRoles,
            IRolePermissionRepository rolePermissions,
            IOrganizationRepository organizations,
            IAreaRepository areas,
            IZoneRepository regions,
            IPermissionRepository permissions)
        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
            RolePermissions = rolePermissions;
            Organizations = organizations;
            Areas = areas;
            Regions = regions;
            Permissions = permissions;
        }

        public IUserRepository Users { get; }

        public IRoleRepository Roles { get; }

        public IUserRoleRepository UserRoles { get; }

        public IRolePermissionRepository RolePermissions { get; }

        public IOrganizationRepository Organizations { get; }

        public IAreaRepository Areas { get; }

        public IZoneRepository Regions { get; }

        public IPermissionRepository Permissions { get; }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
