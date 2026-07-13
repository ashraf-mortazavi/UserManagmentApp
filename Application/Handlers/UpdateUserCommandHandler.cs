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
        private readonly IRegionService _regionService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserService userService,
            IRoleService roleService,
            IOrganizationService organizationService,
            IAreaService areaService,
            IRegionService regionService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _roleService = roleService;
            _organizationService = organizationService;
            _areaService = areaService;
            _regionService = regionService;
            _unitOfWork = unitOfWork;
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

            var roles = await _roleService.GetRolesAsync(request.UserRoleIds, ct);
            if (roles == null || roles.Count == 0)
            {
                response.FailedResult = "نقش مورد نظر یافت نشد!";
                return response;
            }

            if (request.OrganizationId.HasValue)
            {
                var organization = await _organizationService.GetOrganizationAsync(request.OrganizationId.Value, ct);
                if (organization == null)
                {
                    response.FailedResult = "سازمان مورد نظر یافت نشد!";
                    return response;
                }
            }

            if (request.AreaId.HasValue)
            {
                var area = await _areaService.GetAreaAsync(request.AreaId.Value, ct);
                if (area == null)
                {
                    response.FailedResult = "منظقه مورد نظر یافت نشد!";
                    return response;
                }
            }

            if (request.RegionId.HasValue)
            {
                var region = await _regionService.GetRegionAsync(request.RegionId.Value, ct);
                if (region == null)
                {
                    response.FailedResult = "ناحیه مورد نظر یافت نشد!";
                    return response;
                }
            }

            if (request.AccessLevel == AccessLevel.Area && !request.AreaId.HasValue)
            {
                response.FailedResult = "برای سطح دسترسی منطقه، انتخاب منطقه الزامی است!";
                return response;
            }

            if (request.AccessLevel == AccessLevel.Zone && (!request.AreaId.HasValue || !request.RegionId.HasValue))
            {
                response.FailedResult = "برای سطح دسترسی ناحیه، انتخاب منطقه و ناحیه الزامی است!";
                return response;
            }

            if (request.AccessLevel == AccessLevel.Setad && (request.AreaId.HasValue || request.RegionId.HasValue))
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
            user.Position = request.Position;
            user.Description = request.Description;
            user.Enabled = request.Enabled;
            user.AccessLevel = request.AccessLevel;
            user.OrganizationId = request.OrganizationId;
            user.AreaId = request.AreaId;
            user.RegionId = request.RegionId;
            user.UpdatedAt = DateTime.UtcNow;

            _userService.UpdateUser(user, ct);

            var roleResult = await _userService.UpdateUserRolesAsync(user, roles.Select(r => r.Name!).ToList(), ct);
            if (!roleResult.Succeeded)
            {
                response.FailedResult = roleResult.Errors.Select(x => x.Description).FirstOrDefault();
                return response;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            response.Id = user.Id.ToString();
            return response;
        }
    }
}
