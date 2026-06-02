using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs;

public record ResetPasswordRequest(
    [property: Required(ErrorMessage = "ایمیل الزامی است")]
    [property: EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    string Email,

    [property: Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [property: MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    string NewPassword,

    [property: Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [property: Compare("NewPassword", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
    string ConfirmPassword
);


public class ResetPasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string FailedResult { get; set; } = string.Empty;
}
