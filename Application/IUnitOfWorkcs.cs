using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;

namespace ManageUsers.Application
{
    public interface IUnitOfWork
    {
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IUserRoleRepository UserRoles { get; }
        public IRolePermissionRepository RolePermissions { get; }
        public IOrganizationRepository Organizations { get; }

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
