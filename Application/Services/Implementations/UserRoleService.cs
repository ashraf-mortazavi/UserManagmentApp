using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Application.Services.Implementations
{
    public class UserRoleService(IUnitOfWork unitOfWork) : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<IdentityUserRole<string>>> GetUserRole(int userId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.UserRoles.GetUserRolesAsync(userId, cancellationToken);
        }
    }
}
