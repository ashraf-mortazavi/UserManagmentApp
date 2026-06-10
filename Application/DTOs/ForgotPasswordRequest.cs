using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs
{
    public record ForgotPasswordRequest(
        [Required(ErrorMessage ="ایمیل الزامی است")]
        [EmailAddress(ErrorMessage ="فرمت ایمیل نامعتبر است")]string Email)
    { };

    public class ForgotPasswordResponse : BaseResponse
    {
      
    }
}
