using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetUsersQuery(
        string? SerachItem = null,
        int PageNumber = 1,
        int PageSize = 10,
        AccessLevel AccessLevel = AccessLevel.Setad,
        int? AreaId = null,
        int? ZoneId = null,
        int RoleId = 0
    ) : IRequest<GetUsersResponse>;
}
