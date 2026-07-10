using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public class LoginUserCommand(HttpContext context, LoginUserRequest loginUserRequest) : IRequest<LoginUserResponse>
    {
        public HttpContext Context { get; set; } = context;
        public string UserName { get; set; } = loginUserRequest.UserName;
        public string Password { get; set; } = loginUserRequest.Password;
        public string CaptchaId { get; set; } = loginUserRequest.CaptchaId;
        public string CaptchaText { get; set; } = loginUserRequest.CaptchaText;

    }
}
