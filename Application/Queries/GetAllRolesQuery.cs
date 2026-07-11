using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public class GetAllRolesQuery : IRequest<GetAllRolesResponse>
    {
    }
}
