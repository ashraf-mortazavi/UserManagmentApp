using ManageUsers.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.Commands
{
    public sealed record UpdateUserProfileCommand(
        string UserId,
        DateTime? BirthDate,
        string? PhoneNumber,
        IFormFile? Avatar
    ) : IRequest<UpdateUserProfileResponse>;
}
