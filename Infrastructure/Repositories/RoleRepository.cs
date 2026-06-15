using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class RoleRepository (AppDbContext appDbContext) : IRoleRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        public async Task<List<Role>> GetRolesAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            return await _appDbContext.Roles.Where(r => roleIds.Contains(r.Id.ToString())).ToListAsync(cancellationToken);
        }
    }
}
