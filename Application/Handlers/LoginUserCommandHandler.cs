using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Implementations;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace ManageUsers.Application.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserService _userService;
        private readonly ICaptchaService _captchaService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRolePermissionService _rolePermissionService;
        public LoginUserCommandHandler(
            IUserService userService,
            IUserRoleService userRoleService,
            IRolePermissionService rolePermissionService,
            ICaptchaService captchaService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _rolePermissionService = rolePermissionService;
            _captchaService = captchaService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            User? user;
            LoginUserResponse response = new();

            user = await _userService.GetByUserNameAsync(userName: request.UserName, ct: cancellationToken);

            if (user is null)
            {
                response.Status = LoginResultStatus.UserNotFound;
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }
            if (!user.Enabled)
            {
                response.Status = LoginResultStatus.UserDisabled;
                response.FailedResult = "کاربر غیر فعال شده است!";
                return response;
            }

            bool checkPassword = await _userService.CheckPasswordAsync(user: user, password: request.Password);

            if (!checkPassword)
            {
                await _userService.RegisterFailedAttemptAsync(user);
                response.Status = LoginResultStatus.InvalidCredentials;
                response.FailedResult = "رمز عبور کاربر صیحیح نمی باشد!";
                return response;
            }


            bool captchaOk = await _captchaService.ValidateCaptchaAsync(
                request.CaptchaId, request.CaptchaText, cancellationToken);
            if (!captchaOk)
            {
                response.Status = LoginResultStatus.InvalidCaptcha;
                response.FailedResult = "کد امنیتی نامعتبر است!";
                return response;
            }

            if (!CheckLoginPattern(request.UserName, request.Password))
            {
                response.Status = LoginResultStatus.InvalidCredentials;
                response.FailedResult = "فرمت نام کاربری یا رمز عبور نامعتبر است!";
                return response;
            }

            if (await _userService.IsLockedOutAsync(user))
            {
                response.Status = LoginResultStatus.UserLockedOut;
                response.FailedResult =
                    "به دلیل تلاش‌های ناموفق مکرر، حساب کاربری قفل شده است. لطفاً بعداً تلاش کنید";
                return response;
            }

            if (user.PasswordExpiresAt.HasValue && user.PasswordExpiresAt.Value < DateTime.UtcNow)
            {
                response.Status = LoginResultStatus.PasswordExpired;
                response.FailedResult = "رمز عبور شما منقضی شده است. لطفا رمز عبور را تغییر دهید!";
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
            response.IsFirstLogin = user.IsFirstLogin ? true : false;
            response.PhoneNumber = user.PhoneNumber;
            user.IsFirstLogin = false;
            _userService.UpdateUser(user, cancellationToken);

            return response;
        }

        private static bool CheckLoginPattern(string userName, string password)
        {
            const string userPattern = @"^[a-zA-Z0-9_]{3,50}$";
            const string passPattern = @"^.{6,100}$";

            return Regex.IsMatch(userName, userPattern)
                && Regex.IsMatch(password, passPattern);
        }
    }
}
