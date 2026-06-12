using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers;
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordUserCommand, ResetPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public ResetPasswordCommandHandler(
        UserManager<User> userManager,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
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

            var resetResult = await _userManager.ResetPasswordAsync(user, "", request.NewPassword);

            if (!resetResult.Succeeded)
            {
                var error = resetResult.Errors.FirstOrDefault();
                response.FailedResult = error?.Description ?? "توکن نامعتبر یا منقضی شده است!";
                return response;
            }

            await _userManager.UpdateSecurityStampAsync(user);


            IdentityResult identityResult = await _userRepository.SetPasswordByUserIdAsync(user.Id.ToString(), request.NewPassword);

            if (!identityResult.Succeeded)
            {
                response.FailedResult = "در تغییر رمزعبور مشکلی بوجود آمد!";
                return response;
            }


            List<IdentityUserRole<int>> identityUserRoles = await _userRepository.GetUserRole(user.Id);
            Dictionary<int, List<int>> mapRolePermissions = await _userRepository.GetRolePermissions(identityUserRoles, cancellationToken);
            List<int> roleIds = new();
            foreach (var item in mapRolePermissions)
            {
                roleIds.Add(item.Key);
            }

            List<RolePermission> rolePermissions = await _userRepository.GetRolePermisionsByRoleIds(roleIds, cancellationToken);


            CreateTokenContext ctx = new CreateTokenContext
            {
                User = user,
                ActiveRole = true,
                HttpContext = request.HttpContext,
                RolePermissions = rolePermissions,
            };

            JWEToken tokenDTO = await _userRepository.CreateTokenAsync(ctx, cancellationToken: cancellationToken);

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
