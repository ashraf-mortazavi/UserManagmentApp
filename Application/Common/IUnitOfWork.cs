using ManageUsers.Application.Interfaces;

namespace ManageUsers.Application.Common;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }
    IRolePermissionRepository RolePermissions { get; }
    IOrganizationRepository Organizations { get; }
    IAreaRepository Areas { get; }
    IZoneRepository Zones { get; }
    IPermissionRepository Permissions { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
