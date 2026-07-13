using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetRegionsByAreaQueryHandler : IRequestHandler<GetRegionsByAreaQuery, GetRegionsByAreaResponse>
    {
        private readonly IUserService _userService;

        public GetRegionsByAreaQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetRegionsByAreaResponse> Handle(GetRegionsByAreaQuery request, CancellationToken ct)
        {
            var response = new GetRegionsByAreaResponse();
            var regions = await _userService.GetRegionsByAreaAsync(request.AreaId, ct);
            response.Regions = regions.Select(r => new RegionDto(r.Id, r.Name)).ToList();
            return response;
        }
    }
}
