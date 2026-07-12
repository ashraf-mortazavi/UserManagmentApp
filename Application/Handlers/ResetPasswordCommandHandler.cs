using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers;
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordUserCommand, ResetPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;

    public ResetPasswordCommandHandler(
        UserManager<User> userManager,
        IUserService userService,
        IUserRoleService userRoleService,
        IRolePermissionService rolePermissionService)
    {
        _userManager = userManager;
        _userService = userService;
        _userRoleService = userRoleService;
        _rolePermissionService = rolePermissionService;
    }
    public async Task<ResetPasswordResponse> Handle(ResetPasswordUserCommand request, CancellationToken cancellationToken)
    {
        ResetPasswordResponse response = new ();

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }

            if (!user.Enabled)
            {
                response.FailedResult = "حساب کاربری غیرفعال است!";
                return response;
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!resetResult.Succeeded)
            {
                var error = resetResult.Errors.FirstOrDefault();
                response.FailedResult = error?.Description ?? "توکن نامعتبر یا منقضی شده است!";
                return response;
            }

            await _userManager.UpdateSecurityStampAsync(user);


            IdentityResult identityResult = await _userService.SetPasswordByUserIdAsync(user.Id.ToString(), request.NewPassword);

            if (!identityResult.Succeeded)
            {
                response.FailedResult = "در تغییر رمزعبور مشکلی بوجود آمد!";
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
                HttpContext = request.HttpContext,
                RolePermissions = rolePermissions,
            };

            JWEToken tokenDTO = await _userService.CreateTokenAsync(ctx, cancellationToken: cancellationToken);

            response.Success = true;
            response.Message = "رمز عبور با موفقیت تغییر یافت.";
            response.Token = tokenDTO.Token;
            response.RefreshToken = tokenDTO.RefreshToken;

        }
        catch (Exception ex)
        {
            response.FailedResult = "خطایی در پردازش درخواست رخ داد.";
        }

        return response;
    }
}
