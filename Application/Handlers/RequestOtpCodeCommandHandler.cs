using Azure;
using ManageUsers.Application.Commands;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Handlers;

public class RequestOtpCodeCommandHandler : IRequestHandler<RequestOtpCodeCommand, RequestOTPCodeResponse>
{
    private readonly ISmsService _smsService;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;
    private const int OtpExpiryMinutes = 5;
    private readonly string ApplicationName = "مدیریت کاربران";


    public RequestOtpCodeCommandHandler(
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

    public async Task<RequestOTPCodeResponse> Handle(RequestOtpCodeCommand request, CancellationToken cancellationToken)
    {
        RequestOTPCodeResponse requestOTPCodeResponse = new();
        var user = await _userService.GetUserByIdAsync(request.PhoneNumber, cancellationToken);
        if (user is null)
        {
            requestOTPCodeResponse.FailedResult = "کاربر یافت نشد!";
            return requestOTPCodeResponse;
        }
        string otpCode = null;
        if (user.SendDateTimeOTPCode is null || user.SendDateTimeOTPCode < DateTime.Now.AddMinutes(OtpExpiryMinutes))
        {
           otpCode = await _userService.GenerateOtpAsync(user.PhoneNumber!, cancellationToken);
        }


        string smsMessage = SetSMSMessage(user);
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            await _smsService.Send(user.PhoneNumber, smsMessage);
        }
        requestOTPCodeResponse.OTPCode = otpCode;
        requestOTPCodeResponse.OTPCodeValidTimeInMinute = user.SendDateTimeOTPCode.Value.Minute;

        return requestOTPCodeResponse;


    }
    private string SetSMSMessage(User user)
    {
        return @$"کد ورود به اپلیکیشن {ApplicationName}: {user.OTPCode}";
    }
}
