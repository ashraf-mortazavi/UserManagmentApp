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
        var user = await _userService.GetUserByPhoneNumber(request.PhoneNumber, cancellationToken);
        if (user is null)
        {
            requestOTPCodeResponse.FailedResult = "کاربر با این شماره همراه یافت نشد!";
            return requestOTPCodeResponse;
        }
        string otpCode = null;
        if (user.SendDateTimeOTPCode is null || user.SendDateTimeOTPCode < DateTime.Now.AddMinutes(OtpExpiryMinutes))
        {
            otpCode = await _userService.GenerateOtpAsync(request.PhoneNumber!, cancellationToken);
        }


        string smsMessage = SetSMSMessage(otpCode);
        //await _smsService.Send(request.PhoneNumber, smsMessage);
        //if (!string.IsNullOrEmpty(user.PhoneNumber))
        //{
          
        //}
        requestOTPCodeResponse.OTPCode = otpCode;
        requestOTPCodeResponse.OTPCodeValidTimeInMinute = OtpExpiryMinutes;
        user.OTPCode = otpCode;
        _userService.UpdateUser(user, cancellationToken);

        return requestOTPCodeResponse;


    }
    private string SetSMSMessage(string otpCode)
    {
        return @$"کد ورود به اپلیکیشن {ApplicationName}: {otpCode}";
    }
}
