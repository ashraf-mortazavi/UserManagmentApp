using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetZonesQueryHandler : IRequestHandler<GetZonesQuery, GetZonesResponse>
    {
        private readonly IUserService _userService;

        public GetZonesQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetZonesResponse> Handle(GetZonesQuery request, CancellationToken ct)
        {
            var response = new GetZonesResponse();
            var areas = await _userService.GetAllZonesAsync(ct);
            response.Areas = areas.Select(a => new AreaDto(a.Id, a.Name)).ToList();
            return response;
        }
    }
}
