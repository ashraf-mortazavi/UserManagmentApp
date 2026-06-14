using ManageUsers.Application;
using ManageUsers.Application.Interfaces;

namespace ManageUsers.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context,
            IUserRepository users,
            IRoleRepository roles,
            IUserRoleRepository userRoles,
            IRolePermissionRepository rolePermissions,
            IOrganizationRepository organizations
            )

        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
            Organizations = organizations;
            RolePermissions = rolePermissions;
        }

        public IUserRepository Users {get;}

        public IRoleRepository Roles {get;}

        public IUserRoleRepository UserRoles {get;}

        public IRolePermissionRepository RolePermissions {get;}

        public IOrganizationRepository Organizations {get;}

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
           await _context.SaveChangesAsync(ct);
        }
    }
}
