using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs
{
    public class ForgotPasswordRequest()
    {
        [Description("ایمیل")]
        public string Email { get; set; }
    };

    public class ForgotPasswordResponse : BaseResponse
    {
      public string ResetLink { get; set; }
    }
}
