using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public class ChangeUserPasswordRequest
    {
        [Description("رمز فعلی")]
        public string CurrentPassword { get; set; }
        [Description("رمز جدید")]
        public string NewPassword { get; set; }

        [Description("تکرار رمز جدید")]
        public string ConfirmNewPassword { get; set; }
    }


    public sealed class ChangeUserPasswordResponse : BaseResponse
    {
        public string Token { get; set; }
        public string RefereshToken { get; set; }
    }

}
