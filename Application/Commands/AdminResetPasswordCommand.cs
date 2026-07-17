using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands;

public class AdminResetPasswordCommand(string userId) : IRequest<AdminResetPasswordResponse>
{
    public string UserId { get; set; } = userId;
}
