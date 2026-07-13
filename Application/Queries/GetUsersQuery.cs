using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetUsersQuery(
        string? SerachItem = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<GetUsersResponse>;
}
