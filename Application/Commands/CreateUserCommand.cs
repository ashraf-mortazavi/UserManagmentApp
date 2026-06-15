using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public sealed record CreateUserCommand(string FirstName,string LastName,string PhoneNumber,
        string NationalCode,string Email,string PostalCode,
        string UserName, string Password,string? PersonalCode,
        string? Position, string? Description,
        bool Enabled, DateTime CreatedAt,
        int? OrganizationId, int? AreaId, int? RegionId,
        string CreatedById, List<string> UserRoleIds
        ) :  IRequest<CreateUserResponse>
    {};
}