using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public class GetRolesQuery(string userId) : IRequest<GetRolesResponse>
    {
        public string UserId { get; set; } = userId;

    }
}
