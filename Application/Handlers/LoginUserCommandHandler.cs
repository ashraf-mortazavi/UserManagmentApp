using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRolePermissionService _rolePermissionService;

        public LoginUserCommandHandler(IUserService userService, IUserRoleService userRoleService, IRolePermissionService rolePermissionService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _rolePermissionService = rolePermissionService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            User? user;
            LoginUserResponse response = new();

            user = await _userService.GetByUserNameAsync(userName: request.UserName, ct: cancellationToken);

            if (user is null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }
            if (!user.Enabled)
            {
                response.FailedResult = "کاربر غیر فعال است!";
                return response;
            }

            bool checkPassword = await _userService.CheckPasswordAsync(user: user, password: request.Password);

            if (!checkPassword)
            {
                response.FailedResult = "رمز ورود اشتباه است!";
                return response;
            }
            List<IdentityUserRole<string>> identityUserRoles = await _userRoleService.GetUserRole(user.Id, cancellationToken: cancellationToken);
            Dictionary<string, List<string>> mapRolePermissions = await _rolePermissionService.GetRolePermissionsByUserRolesAsync(identityUserRoles, cancellationToken);
            List<string> roleIds = new();
            foreach (var item in mapRolePermissions)
            {
                roleIds.Add(item.Key);
            }

            List<RolePermission> rolePermissions = await _rolePermissionService.GetRolePermisionsByRoleIdsAsync(roleIds, cancellationToken);

            CreateTokenContext ctx = new CreateTokenContext
            {
                User = user,
                ActiveRole = true,
                HttpContext = request.Context,
                RolePermissions = rolePermissions,
            };

            // JWT Code
            JWEToken tokenDTO = await _userService.CreateTokenAsync(ctx, cancellationToken: cancellationToken);
       
            response.Token = tokenDTO.Token;
            response.RefreshToken = tokenDTO.RefreshToken;

            return response;
        }
    }
}
