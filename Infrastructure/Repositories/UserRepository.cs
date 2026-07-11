using ManageUsers.Application.Common;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
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
