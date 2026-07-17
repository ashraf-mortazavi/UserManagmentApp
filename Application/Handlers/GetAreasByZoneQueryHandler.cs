using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetAreasByZoneQueryHandler : IRequestHandler<GetAreasByZoneQuery, GetAreasByZoneResponse>
    {
        private readonly IAreaService _areaService;

        public GetAreasByZoneQueryHandler(IAreaService areaService)
        {
            _areaService = areaService;
        }

        public async Task<GetAreasByZoneResponse> Handle(GetAreasByZoneQuery request, CancellationToken ct)
        {
            GetAreasByZoneResponse response = new();
            var areas = await _areaService.GetAreasByZone(request.ZoneId, ct);
            response.Areas = areas.Select(r => new AreaDto(r.Id, r.Name)).ToList();
            return response;
        }
    }
}
