using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class PermissionRepository(AppDbContext appDbContext) : IPermissionRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task<List<Permission>> GetPermisionsByPermissionIds(List<string> permissionIds, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Permissions
                .Where(rp => permissionIds.Contains(rp.Id.ToString()))
                .ToListAsync(cancellationToken);
        }
    }
}
