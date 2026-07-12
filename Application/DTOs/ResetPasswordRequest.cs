using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs;

public class ResetPasswordRequest
{
    [Description("ایمیل")]
    public string Email { get; set; }
    [Description("پسورد جدید")]
    public string NewPassword { get; set; }
    [Description("تایید پسورد جدید")]
    public string ConfirmPassword { get; set; }
}


public class ResetPasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string FailedResult { get; set; } = string.Empty;
}
