using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;




namespace ManageUsers.Application.Handlers
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly IAreaService _areaService;
        private readonly IZoneService _zoneService;

        public CreateUserCommandHandler(IRoleService roleService, IUserService userService, IOrganizationService organizationService, IAreaService areaService, IZoneService zoneService)
        {
            _roleService = roleService;
            _userService = userService;
            _organizationService = organizationService;
            _areaService = areaService;
            _zoneService = zoneService;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken ct)
        {
            CreateUserResponse userResponse = new();
            Role? role = await _roleService.GetRoleAsync(request.UserRoleId, ct);

            if (role is null)
            {
                userResponse.FailedResult = "نقش مورد نظر یافت نشد!";
                return userResponse;
            }
            User? existingUser = await _userService.GetUserByNationalCodeAsync(request.NationalCode, ct);

            if (existingUser != null)
            {
                userResponse.FailedResult = "کد ملی کاربر تکراری است!";
                return userResponse;
            }

            if (request.ZoneId.HasValue)
            {
                var existZone = await _zoneService.GetZoneAsync(request.ZoneId!.Value, ct);
                if (existZone == null)
                {
                    userResponse.FailedResult = "منطقه مورد نظر یافت نشد!";
                    return userResponse;
                }
            }

            if (request.AreaId.HasValue)
            {
                var existArea = await _areaService.GetAreaAsync(request.AreaId!.Value, ct);
                if (existArea == null)
                {
                    userResponse.FailedResult = "ناحیه مورد نظر یافت نشد!";
                    return userResponse;
                }
            }

            if (request.AccessLevel == AccessLevel.Area && (!request.AreaId.HasValue || !request.ZoneId.HasValue))
            {
                userResponse.FailedResult = "برای سطح دسترسی ناحیه، انتخاب منطقه و ناحیه الزامی است!";
                return userResponse;
            }

            if (request.AccessLevel == AccessLevel.Zone && !request.ZoneId.HasValue)
            {
                userResponse.FailedResult = "برای سطح دسترسی منطقه، انتخاب منطقه  الزامی است!";
                return userResponse;
            }

            if (request.AccessLevel == AccessLevel.Setad && (request.AreaId.HasValue || request.ZoneId.HasValue))
            {
                userResponse.FailedResult = "برای سطح دسترسی ستاد نباید منطقه یا ناحیه انتخاب شود!";
                return userResponse;
            }


            User newUser = new User();
            newUser.FirstName = request.FirstName;
            newUser.LastName = request.LastName;
            newUser.NationalCode = request.NationalCode;
            newUser.PhoneNumber = request.PhoneNumber;
            newUser.Email = request.Email;
            newUser.PostalCode = request.PostalCode;
            newUser.ZonId = request.ZoneId;
            newUser.AreaId = request.AreaId;
            newUser.PersonalCode = request.PersonalCode;
            newUser.CreatedById = request.CreatedById;
            newUser.Enabled = request.Enabled;
            newUser.AccessLevel = request.AccessLevel;
            newUser.IsFirstLogin = true;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UserName = request.UserName;
            newUser.BirthDate = request.BirthDate;
            newUser.SetadName = request.SetadName;

            var result = await _userService.AssignUserRoleAsync(user: newUser, request.Password, role.Name!, cancellationToken: ct);
            if (!result.Succeeded)
            {
                userResponse.FailedResult = result.Errors.Select(x => x.Description).FirstOrDefault();
                return userResponse;
            }
            return userResponse;
        }
    }

}
