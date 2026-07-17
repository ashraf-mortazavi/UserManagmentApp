using ManageUsers.Application.Common.Utilities;
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
            GetUsersResponse response = new ();

            List<User> users = await _userService.GetAllUsersAsync(
                request.SerachItem, request.PageNumber, request.PageSize,
                request.AccessLevel, request.AreaId, request.ZoneId, request.RoleId, ct);

            response.Users = users.Select(u => new UserDto
            {
                Id = u.Id.ToString(),
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                Enabled = u.Enabled,
                AccessLevel = u.AccessLevel,
                AreaId = u.AreaId,
                ZoneId = u.ZonId,
                AreaName = u.Area?.Name,
                ZoneName = u.Zone?.Name,
                BirthDateShamsi = u.BirthDate.ToShamsi(),
                AvatarUrl = u.AvatarUrl,
                AdminGeneratedPassword = null,
                CreatedAt = u.CreatedAt,
                RoleName = u.UserRoles.FirstOrDefault()?.Role?.Name
            }).ToList();

            response.TotalCount = await _userService.GetTotalCountAsync(
                request.SerachItem, request.AccessLevel, request.AreaId, request.ZoneId, request.RoleId, ct);

            return response;
        }
    }
}
