using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public class LoginUserRequest
    {
        [Description("نام کاربری")]
        public string UserName { get; set; }

        [Description("رمز عبور")]
        public string Password { get; set; }
        public string CaptchaId { get; set; } = string.Empty;
        [Description("کد امنیتی")]
        public string CaptchaText { get; set; } = string.Empty;
    }

    public class LoginUserResponse : BaseResponse
    {
       
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
        public bool RequiresOtp { get; set; }
        public  bool IsFirstLogin { get; set; }
        public LoginResultStatus Status { get; set; }

    }
    public enum LoginResultStatus
    {
        Success,
        InvalidCaptcha,
        UserNotFound,
        UserDisabled,
        UserLockedOut,
        InvalidCredentials,
        PasswordExpired,
        FirstLoginPasswordChangeRequired
    }
}
