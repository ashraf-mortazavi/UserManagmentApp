using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Implementations;

public class UserService (IUnitOfWork unitOfWork, UserManager<User> userManager,
    RoleManager<Role> roleManager) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<User> _usermanager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;

    public Task<User> AssignUserRolesAsync(User user, string password, List<string> roles, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        throw new NotImplementedException();
    }

    public Task<JWEToken> CreateTokenAsync(CreateTokenContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> SetPasswordByUserIdAsync(string userId, string newPassword)
    {
        throw new NotImplementedException();
    }
}
