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
        private readonly IRegionService _regionService;

        public GetUserByIdQueryHandler(IUserService userService, IOrganizationService organizationService, IAreaService areaService, IRegionService regionService)
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
            Organization? organization = null;
            if (user.OrganizationId.HasValue)
            { 
                organization = await _organizationService.GetOrganizationAsync(user.OrganizationId.Value, ct);
            }
            Area? area = null;
            if (user.AreaId.HasValue)
            {
                area = await _areaService.GetAreaAsync(user.AreaId.Value, ct);
            }
            Region? region = null;
            if (user.RegionId.HasValue)
            {
                region = await _regionService.GetRegionAsync(user.RegionId.Value, ct);
            }

            var roleIds = await _userService.GetUserRoleIdsAsync(user.Id, ct);

            response.Id = user.Id.ToString();
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.UserName = user.UserName!;
            response.PhoneNumber = user.PhoneNumber!;
            response.NationalCode = user.NationalCode;
            response.Email = user.Email;
            response.PostalCode = user.PostalCode;
            response.PersonalCode = user.PersonalCode;
            response.Position = user.Position;
            response.Enabled = user.Enabled;
            response.AreaName = area is null ? string.Empty : area.Name;
            response.AreaId = area is null ? 0 : area.Id;
            response.RegionName = region is null ? string.Empty : region.Name;
            response.RegionId = region is null ? 0 : region.Id;
            response.UserRoleIds = roleIds;
            response.CreatedAt = user.CreatedAt;
            response.UpdatedAt = user.UpdatedAt;

            return response;
        }
    }
}
