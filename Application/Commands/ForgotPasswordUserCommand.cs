using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public class ForgotPasswordUserCommand(string email, HttpContext context) : IRequest<ForgotPasswordResponse>
    {
        public string Email { get; set; } = email;
        public HttpContext Context { get; set; } = context;

    }
}
