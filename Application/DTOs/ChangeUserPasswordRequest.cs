using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public record ChangeUserPasswordRequest(
      string CurrentPassword,
      string NewPassword,
      string ConfirmNewPassword
  );


    public sealed class ChangeUserPasswordResponse : BaseResponse
    {
        public string Token { get; set; }
        public string RefereshToken { get; set; }
    }
  
}
