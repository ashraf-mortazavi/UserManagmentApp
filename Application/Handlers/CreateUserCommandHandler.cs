using Azure;
using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;




namespace ManageUsers.Application.Handlers
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly IAreaService _areaService;
        private readonly IRegionService _regionService;

        public CreateUserCommandHandler(IRoleService roleService, IUserService userService, IOrganizationService organizationService, IAreaService areaService, IRegionService regionService)
        {
            _roleService = roleService;
            _userService = userService;
            _organizationService = organizationService;
            _areaService = areaService;
            _regionService = regionService;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken ct)
        {
            CreateUserResponse userResponse = new();
            List<Role?> roles = await _roleService.GetRolesAsync(request.UserRoleIds, ct);

            if (!roles.Any() || roles is null || roles.Count == 0)
            {
                userResponse.FailedResult = "نقش مورد نظر یافت نشد!";
                return userResponse;
            }
            var existingUser = await _userService.GetUserByNationalCodeAsync(request.NationalCode, ct);

            if (existingUser != null)
            {
                userResponse.FailedResult = "کد ملی کاربر تکراری است!";
                return userResponse;
            }

            Organization organization = null;
            if (request.OrganizationId.HasValue)
            {
                var existOrganization = await _organizationService.GetOrganizationAsync(request.OrganizationId!.Value, ct);
                if (existOrganization == null)
                {
                    userResponse.FailedResult = "سازمان مورد نظر یافت نشد!";
                    return userResponse;
                }
            }

            if (request.AreaId.HasValue)
            {
                var existArea = await _areaService.GetAreaAsync(request.AreaId!.Value, ct);
                if (existArea == null)
                {
                    userResponse.FailedResult = "منظقه مورد نظر یافت نشد!";
                    return userResponse;
                }
            }


            if (request.RegionId.HasValue)
            {
                var existRegion = await _regionService.GetRegionAsync(request.RegionId!.Value, ct);
                if (existRegion == null)
                {
                    userResponse.FailedResult = "ناحیه مورد نظر یافت نشد!";
                    return userResponse;
                }
            }
            User newUser = new User();
            newUser.FirstName = request.FirstName;
            newUser.LastName = request.LastName;
            newUser.NationalCode = request.NationalCode;
            newUser.PhoneNumber = request.PhoneNumber;
            newUser.Email = request.Email;
            newUser.PostalCode = request.PostalCode;
            newUser.RegionId = request.RegionId;
            newUser.AreaId = request.AreaId;
            newUser.Description = request.Description;
            newUser.OrganizationId = request.OrganizationId;
            newUser.PersonalCode = request.PersonalCode;
            newUser.Position = request.Position;
            newUser.CreatedById = request.CreatedById;
            newUser.Enabled = true;
            newUser.IsFirstLogin = true;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UserName = request.UserName;

            var result = await _userService.AssignUserRolesAsync(user: newUser, request.Password, roles.Select(x => x.Name!).ToList(), cancellationToken: ct);
            if (!result.Succeeded)
            {
                userResponse.FailedResult = result.Errors.Select(x => x.Description).FirstOrDefault();
                return userResponse;
            }
            return userResponse;
        }
    }

}
