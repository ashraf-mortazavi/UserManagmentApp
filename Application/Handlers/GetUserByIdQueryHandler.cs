using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly IAreaService _areaService;
        private readonly IZoneService _regionService;

        public GetUserByIdQueryHandler(IUserService userService, IOrganizationService organizationService, IAreaService areaService, IZoneService regionService)
        {
            _userService = userService;
            _organizationService = organizationService;
            _areaService = areaService;
            _regionService = regionService;

        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var response = new GetUserByIdResponse();

            var user = await _userService.GetUserByIdWithRolesAsync(request.UserId, ct);

            if (user == null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }
            Area? area = null;
            if (user.AreaId.HasValue)
            {
                area = await _areaService.GetAreaAsync(user.AreaId.Value, ct);
            }
            Zone? zone = null;
            if (user.ZonId.HasValue)
            {
                zone = await _regionService.GetZoneAsync(user.ZonId.Value, ct);
            }

            string roleId = await _userService.GetUserRoleIdsAsync(user.Id, ct);

            response.Id = user.Id.ToString();
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.UserName = user.UserName!;
            response.PhoneNumber = user.PhoneNumber!;
            response.NationalCode = user.NationalCode;
            response.Email = user.Email;
            response.PostalCode = user.PostalCode;
            response.PersonalCode = user.PersonalCode;
            response.Enabled = user.Enabled;
            response.AccessLevelName = user.AccessLevel.ToString();
            response.AreaName = area is null ? string.Empty : area.Name;
            response.AreaId = area is null ? 0 : area.Id;
            response.ZoneName = zone is null ? string.Empty : zone.Name;
            response.ZoneId = zone is null ? 0 : zone.Id;
            response.BirthDate = user.BirthDate;
            response.BirthDateShamsi = user.BirthDate.ToShamsi();
            response.AvatarUrl = user.AvatarUrl;
            response.RoleId = roleId;
            response.CreatedAt = user.CreatedAt;
            response.UpdatedAt = user.UpdatedAt;

            return response;
        }
    }
}
