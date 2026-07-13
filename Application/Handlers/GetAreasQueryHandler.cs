using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetAreasQueryHandler : IRequestHandler<GetAreasQuery, GetAreasResponse>
    {
        private readonly IUserService _userService;

        public GetAreasQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetAreasResponse> Handle(GetAreasQuery request, CancellationToken ct)
        {
            var response = new GetAreasResponse();
            var areas = await _userService.GetAllAreasAsync(ct);
            response.Areas = areas.Select(a => new AreaDto(a.Id, a.Name)).ToList();
            return response;
        }
    }
}
