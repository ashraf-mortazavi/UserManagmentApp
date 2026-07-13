using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public record GetAreasQuery : IRequest<GetAreasResponse>;
}
