using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public record ChangeUserPasswordRequest(
      string CurrentPassword,
      string NewPassword,
      string ConfirmNewPassword
  );


    public sealed class ChangeUserPasswordResponse
    {
        public string Token { get; set; }
        public string RefereshToken { get; set; }
        public string FailedResult { get; set; }
    }
  
}
