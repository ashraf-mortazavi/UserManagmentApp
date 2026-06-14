
using ManageUsers.Application;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Infrastructure.Persistence;
using System.Threading;

namespace ManageUsers.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IUserRoleRepository UserRoles { get; }
    public IRolePermissionRepository RolePermissions { get; }
    public IOrganizationService Organizations { get; }

    public UnitOfWork(AppDbContext appDbContext,
        IUserRepository users,
        IRoleRepository roles,
        IUserRoleRepository userRoles,
        IRolePermissionRepository rolePermissions,
        IOrganizationService organizations
        )

    {
        _appDbContext = appDbContext;
        Users = users;
        Roles = roles;
        UserRoles = userRoles;
        RolePermissions = rolePermissions;
        Organizations = organizations;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _ = await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}
