using ManageUsers.Application.DTOs;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetUsersQuery(
        string? SerachItem = null,
        int PageNumber = 1,
        int PageSize = 10,
        AccessLevel CallerAccessLevel = AccessLevel.Setad,
        int? CallerAreaId = null,
        int? CallerRegionId = null
    ) : IRequest<GetUsersResponse>;
}
