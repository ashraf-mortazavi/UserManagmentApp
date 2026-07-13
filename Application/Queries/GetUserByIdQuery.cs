using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetUserByIdQuery(string UserId) : IRequest<GetUserByIdResponse>;
}
