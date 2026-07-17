using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;

namespace ManageUsers.Application.Handlers;

public sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public GetUserProfileQueryHandler(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    public async Task<GetUserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        GetUserProfileResponse response = new();

        var user = await _userService.GetUserByIdWithRolesAsync(request.UserId, ct);
        if (user == null)
        {
            response.FailedResult = "کاربر یافت نشد!";
            return response;
        }

        string roleId = await _userService.GetUserRoleIdsAsync(user.Id, ct);
        string? roleName = null;
        if (!string.IsNullOrEmpty(roleId))
        {
            Role role = await _roleService.GetRoleAsync(roleId, ct);
            roleName = role?.Name;
        }

        response.FirstName = user.FirstName;
        response.LastName = user.LastName;
        response.PhoneNumber = user.PhoneNumber;
        response.Email = user.Email;
        response.AvatarUrl = user.AvatarUrl;
        response.RoleName = roleName;
        response.AccessLevel = user.AccessLevel;

        return response;
    }
}
