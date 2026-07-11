using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public record ChangeUserPasswordRequest(
      [Description]string CurrentPassword,
      [Description] string NewPassword,
      [Description] string ConfirmNewPassword
  );


    public sealed class ChangeUserPasswordResponse : BaseResponse
    {
        public string Token { get; set; }
        public string RefereshToken { get; set; }
    }
  
}
