using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.Commands
{
    public sealed record CreateUserCommand(string FirstName,string LastName,string PhoneNumber,
        string NationalCode,string Email,string PostalCode,
        string UserName, string Password,string? PersonalCode,
        bool Enabled, DateTime CreatedAt,
        AccessLevel AccessLevel,
        int? AreaId, int? ZoneId, string? SetadName,
        string CreatedById, string UserRoleId,
        DateTime? BirthDate
        ) :  IRequest<CreateUserResponse>
    {};
}