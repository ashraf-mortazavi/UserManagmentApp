using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public class LoginUserCommand(HttpContext context, LoginUserRequest loginUserRequest) : IRequest<LoginUserResponse>
    {
        public HttpContext Context { get; set; } = context;
        public string UserName { get; set; } = loginUserRequest.UserName;
        public string Password { get; set; } = loginUserRequest.Password;
    }
}
