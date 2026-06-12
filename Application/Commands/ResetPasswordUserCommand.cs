using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public class ResetPasswordUserCommand(string email, string token, string newPassword, HttpContext httpContext) : IRequest<ResetPasswordResponse>
    {
        public string Email { get; set; } = email;
        public string NewPassword { get; set; } = newPassword;
        public HttpContext HttpContext { get; set; } = httpContext;
    }
}
