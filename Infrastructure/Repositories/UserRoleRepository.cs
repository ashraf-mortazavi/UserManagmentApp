using ManageUsers.Application.Interfaces;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;
        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<IdentityUserRole<int>>> GetUserRolesAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.UserRoles.Where(u => u.UserId == userId)
               .Select(ur => new IdentityUserRole<int>
               {
                   UserId = userId,
                   RoleId = ur.RoleId
               }).ToListAsync(cancellationToken);
        }
    }
}
