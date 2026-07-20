using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

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
        bool Enabled,
        bool IsApprovedByAdmin,
        AccessLevel AccessLevel,
        int? AreaId,
        int? ZoneId,
        string RoleId,
        DateTime? BirthDate
    ) : IRequest<UpdateUserResponse>;
}
