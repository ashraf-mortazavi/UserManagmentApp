using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Commands
{
    public sealed record CreateUserCommand(string FirstName,string LastName,string PhoneNumber,
     string NationalCode,string Email,string PostalCode,
     string UserName, string Password,Guid CreatedById, List<string> UserRoles,
     bool Enabled,DateTime CreatedAt) :  IRequest<CreateUserResponse>
    {

    };
}