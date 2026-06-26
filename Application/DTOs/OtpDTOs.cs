using System.ComponentModel;

namespace ManageUsers.Application.DTOs;

public class RequestOTPCode
{
    [Description("تلفن")]
    public string PhoneNumber { get; set; }
}

public class RequestOTPCodeResponse : BaseResponse
{
    [Description("کد OTP")]
    public string OTPCode { get; set; }

    [Description("مدت اعتبار کد OTP")]
    public int OTPCodeValidTimeInMinute { get; set; } = 2;
}

