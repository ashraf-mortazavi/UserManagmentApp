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

        public CreateUserCommandHandler(IRoleService roleService, IUserService userService, IOrganizationService organizationService)
        {
            _roleService = roleService;
            _userService = userService;
            _organizationService = organizationService;
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
                organization = await _organizationService.GetOrganizationAsync(request.OrganizationId!.Value, ct);
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
            newUser.OrganizationId = organization != null ? organization.Id : null;
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
