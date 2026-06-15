using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ManageUsers.Application.Handlers;
public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, GetRolesResponse>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;

    public GetRolesQueryHandler(IUserService userService, IRoleService roleService, IUserRoleService userRoleService)
    {
        _userService = userService;
        _roleService = roleService;
        _userRoleService = userRoleService; 
    }
    public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        GetRolesResponse roleResponse = new();
        User user = await _userService.GetUserByIdAsync(request.UserId, ct: cancellationToken);
        if (user is null)
        {

            roleResponse.FailedResult = "کاربر مورد نظر یافت نشد!";
            return roleResponse;
        }

        List<IdentityUserRole<string>> identityUserRoles = await _userRoleService.GetUserRole(user.Id, cancellationToken: cancellationToken);
        List<string> userRoleIds = identityUserRoles.Select(x => x.RoleId).ToList();

        List<Role> allRoles = await _roleService.GetAllRolesAsync(cancellationToken);

        Dictionary<string, Role> roleDictionary = allRoles.ToDictionary(r => r.Id.ToString(), r => r);

        List<RoleItem> roleItems = new List<RoleItem>();

        foreach (string userRoleId in userRoleIds)
        {
            if (!roleDictionary.ContainsKey(userRoleId))
                continue;

            // Create LinkedList and add nodes
            LinkedList<Role> roleChain = new LinkedList<Role>();
            string? currentId = userRoleId;

            while (!string.IsNullOrEmpty(currentId) && roleDictionary.ContainsKey(currentId))
            {
                Role currentRole = roleDictionary[currentId];
                roleChain.AddLast(currentRole);
                currentId = currentRole.NextLowerRoleId.ToString();
            }

            foreach (var role in roleChain)
            {
                var nextRole = roleChain.Find(role)?.Next?.Value;
                var roleItem = new RoleItem()
                {
                    Id = role.Id,
                    Name = role.Name,
                    NextRoleName = nextRole?.Name ?? string.Empty
                };
                roleItems.Add(roleItem);
            }
        }

        return new GetRolesResponse
        {
            Items = roleItems,
            FailedResult = null
        };
    }
}

