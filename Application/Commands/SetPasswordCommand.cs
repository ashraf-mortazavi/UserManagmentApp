using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public sealed record SetPasswordCommand(
      string UserName,
      string Password
  ) : IRequest<SetPasswordResponse>;
}
