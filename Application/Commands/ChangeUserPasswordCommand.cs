using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public class ChangeUserPasswordCommand(string userId, HttpContext context, ChangeUserPasswordRequest changeUserPasswordRequest) : IRequest<ChangeUserPasswordResponse>
    {
        public string UserId { get; set; } = userId;
        public HttpContext Context { get; set; } = context;
        public string CurrentPassword { get; set; } = changeUserPasswordRequest.CurrentPassword;
        public string NewPassword { get; set; } = changeUserPasswordRequest.NewPassword;
    }
}
