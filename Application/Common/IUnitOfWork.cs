
using ManageUsers.Application.Interfaces;

namespace ManageUsers.Application.Common;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }
    IRolePermissionRepository RolePermissions { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);
}