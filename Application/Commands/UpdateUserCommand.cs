using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public sealed record UpdateUserCommand(
        string UserId,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string NationalCode,
        string? Email,
        string? PostalCode,
        string? PersonalCode,
        string? Position,
        bool Enabled,
        AccessLevel AccessLevel,
        int? AreaId,
        int? RegionId,
        List<string> UserRoleIds
    ) : IRequest<UpdateUserResponse>;
}
