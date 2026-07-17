using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetAreasByZoneQuery(int ZoneId) : IRequest<GetAreasByZoneResponse>;
}
