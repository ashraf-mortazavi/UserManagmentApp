using ManageUsers.Application.Commands;
using ManageUsers.Application.Common;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Handlers
{
    public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IOrganizationService _organizationService;
        private readonly IAreaService _areaService;
        private readonly IZoneService _zoneService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public UpdateUserCommandHandler(
            IUserService userService,
            IRoleService roleService,
            IOrganizationService organizationService,
            IAreaService areaService,
            IZoneService regionService,
            IUnitOfWork unitOfWork,
            IFileService fileService)
        {
            _userService = userService;
            _roleService = roleService;
            _organizationService = organizationService;
            _areaService = areaService;
            _zoneService = regionService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            var response = new UpdateUserResponse();

            var user = await _userService.GetUserByIdAsync(request.UserId, ct);
            if (user == null)
            {
                response.FailedResult = "کاربر یافت نشد!";
                return response;
            }

            Role? role = await _roleService.GetRoleAsync(request.RoleId, ct);
            if (role == null)
            {
                response.FailedResult = "نقش مورد نظر یافت نشد!";
                return response;
            }

            if (request.AreaId.HasValue)
            {
                var area = await _areaService.GetAreaAsync(request.AreaId.Value, ct);
                if (area == null)
                {
                    response.FailedResult = "ناحیه مورد نظر یافت نشد!";
                    return response;
                }
            }

            if (request.ZoneId.HasValue)
            {
                var region = await _zoneService.GetZoneAsync(request.ZoneId.Value, ct);
                if (region == null)
                {
                    response.FailedResult = "منطقه مورد نظر یافت نشد!";
                    return response;
                }
            }

            if (request.AccessLevel == AccessLevel.Zone && !request.ZoneId.HasValue)
            {
                response.FailedResult = "برای سطح دسترسی منطقه، انتخاب منطقه الزامی است!";
                return response;
            }

            if (request.AccessLevel == AccessLevel.Area && (!request.AreaId.HasValue || !request.ZoneId.HasValue))
            {
                response.FailedResult = "برای سطح دسترسی ناحیه، انتخاب منطقه و ناحیه الزامی است!";
                return response;
            }

            if (request.AccessLevel == AccessLevel.Setad && (request.AreaId.HasValue || request.ZoneId.HasValue))
            {
                response.FailedResult = "برای سطح دسترسی ستاد نباید منطقه یا ناحیه انتخاب شود!";
                return response;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.NationalCode = request.NationalCode;
            user.Email = request.Email;
            user.PostalCode = request.PostalCode;
            user.PersonalCode = request.PersonalCode;
            user.Enabled = request.Enabled;
            user.AccessLevel = request.AccessLevel;
            user.AreaId = request.AreaId;
            user.ZonId = request.ZoneId;
            user.BirthDate = request.BirthDate;
            user.UpdatedAt = DateTime.UtcNow;

            _userService.UpdateUser(user, ct);

            var roleResult = await _userService.UpdateUserRolesAsync(user, role.Name!, ct);
            if (!roleResult.Succeeded)
            {
                response.FailedResult = roleResult.Errors.Select(x => x.Description).FirstOrDefault();
                return response;
            }


            response.Id = user.Id.ToString();
            return response;
        }
    }
}
