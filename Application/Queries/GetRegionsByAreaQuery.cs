using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetRegionsByAreaQuery(int AreaId) : IRequest<GetRegionsByAreaResponse>;
}
