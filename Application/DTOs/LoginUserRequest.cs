using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public class LoginUserRequest
    {
        [Description("نام کاربری")]
        public string UserName { get; set; }

        [Description("رمزعبور")]
        public string Password { get; set; }
    }

    public class LoginUserResponse : BaseResponse
    {
        [Description("توکن")]
        public string Token { get; set; }
        [Description("توکن مجدد")]
        public string RefreshToken { get; set; }
    }
}
