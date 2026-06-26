using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public class LoginUserRequest
    {
        [Description("نام کاربری")]
        public string UserName { get; set; }

        [Description("رمز عبور")]
        public string Password { get; set; }
    }

    public class LoginUserResponse : BaseResponse
    {
       
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
        public bool RequiresOtp { get; set; }

    }
}
