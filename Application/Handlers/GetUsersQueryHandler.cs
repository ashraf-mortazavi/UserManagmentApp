using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
    {
        private readonly IUserService _userService;

        public GetUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken ct)
        {
            var response = new GetUsersResponse();
            
            List<User> users = await _userService.GetAllUsersAsync(request.SerachItem, request.PageNumber, request.PageSize, ct);

                response.Users = users.Select(u => new UserDto
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    Enabled = u.Enabled,
                    CreatedAt = u.CreatedAt
                }).ToList();

                response.TotalCount = await _userService.GetTotalCountAsync(request.SerachItem, ct);

            return response;
        }
    }
}
