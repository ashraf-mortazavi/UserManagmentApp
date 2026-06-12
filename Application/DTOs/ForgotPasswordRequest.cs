using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs
{
    public record ForgotPasswordRequest(string Email)
    { };

    public class ForgotPasswordResponse : BaseResponse
    {
      
    }
}
