using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetAreasByZoneQuery(int AreaId) : IRequest<GetAreasByZoneResponse>;
}
