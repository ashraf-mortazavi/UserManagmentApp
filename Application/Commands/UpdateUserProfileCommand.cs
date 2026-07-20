using ManageUsers.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.Commands
{
    public sealed record UpdateUserProfileCommand(
        string UserId,
        string? Email,
        string? PhoneNumber,
        IFormFile? Avatar
    ) : IRequest<UpdateUserProfileResponse>;
}
