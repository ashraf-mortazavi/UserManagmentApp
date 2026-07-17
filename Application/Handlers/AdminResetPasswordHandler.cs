using System.Security.Cryptography;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers;

public sealed class AdminResetPasswordHandler : IRequestHandler<AdminResetPasswordCommand, AdminResetPasswordResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;

    public AdminResetPasswordHandler(UserManager<User> userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    public async Task<AdminResetPasswordResponse> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        AdminResetPasswordResponse response = new();

        var user = await _userService.GetUserByIdAsync(request.UserId);
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

        string generatedPassword = GenerateRandomPassword();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, token, generatedPassword);

        if (!resetResult.Succeeded)
        {
            var error = resetResult.Errors.FirstOrDefault();
            response.FailedResult = error?.Description ?? "بازنشانی رمز عبور با مشکل مواجه شد!";
            return response;
        }

        await _userManager.UpdateSecurityStampAsync(user);

        response.GeneratedPassword = generatedPassword;
        return response;
    }

    private static string GenerateRandomPassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";

        char[] password = new char[12];

        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        string allChars = upper + lower + digits + special;
        for (int i = 4; i < password.Length; i++)
        {
            password[i] = allChars[RandomNumberGenerator.GetInt32(allChars.Length)];
        }

        return new string(password);
    }
}
