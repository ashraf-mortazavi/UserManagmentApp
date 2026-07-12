using Azure;
using ManageUsers.Application.Commands;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCodeCommand, VerifyOtpResponse>
{
    private readonly ISmsService _smsService;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;
    private const int OtpExpiryMinutes = 5;
    private readonly string ApplicationName = "مدیریت کاربران";


    public VerifyOtpCommandHandler(
        ISmsService smsService,
        IUserService userService,
        IUserRoleService userRoleService,
        IRolePermissionService rolePermissionService)
    {
        _smsService = smsService;
        _userService = userService;
        _userRoleService = userRoleService;
        _rolePermissionService = rolePermissionService;
    }

    public async Task<VerifyOtpResponse> Handle(VerifyOtpCodeCommand request, CancellationToken cancellationToken)
    {
        VerifyOtpResponse verifyOtpResponse = new();
        var user = await _userService.GetUserByPhoneNumber(request.PhoneNumber, cancellationToken);

        if (user is null)
        {
            verifyOtpResponse.FailedResult = "کاربر با این شماره همراه یافت نشد!";
            return verifyOtpResponse;
        }

        if (!string.Equals(user.OTPCode, request.OTPCode, StringComparison.Ordinal))
        {
            verifyOtpResponse.FailedResult = "کد وارد شده صحیح نمی باشد!";
            return verifyOtpResponse;
        }

        return new VerifyOtpResponse() { IsVerified = true };
    }

}
