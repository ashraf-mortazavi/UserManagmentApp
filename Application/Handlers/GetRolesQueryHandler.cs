using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Queries;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ManageUsers.Application.Handlers;
public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, GetRolesResponse>
{
    private readonly IUserRepository _userRepository;

    public GetRolesQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        GetRolesResponse roleResponse = new();
        User user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            roleResponse= new GetRolesResponse()
            {
                FailedResult = "کاربر یافت نشد!"
            };
            return roleResponse;
        }

        List<IdentityUserRole<int>> identityUserRoles = await _userRepository.GetUserRole(user.Id);
        List<int> userRoleIds = identityUserRoles.Select(x => x.RoleId).ToList();

        List<Role> allRoles = await _userRepository.GetAllRolesAsync(cancellationToken);

        Dictionary<int, Role> roleDictionary = allRoles.ToDictionary(r => r.Id, r => r);

        List<RoleItem> roleItems = new List<RoleItem>();

        foreach (int userRoleId in userRoleIds)
        {
            if (!roleDictionary.ContainsKey(userRoleId))
                continue;

            // Create LinkedList and add nodes
            LinkedList<Role> roleChain = new LinkedList<Role>();
            int? currentId = userRoleId;

            while (currentId.HasValue && roleDictionary.ContainsKey(currentId.Value))
            {
                Role currentRole = roleDictionary[currentId.Value];
                roleChain.AddLast(currentRole);
                currentId = currentRole.NextLowerRoleId;
            }

            foreach (var role in roleChain)
            {
                var nextRole = roleChain.Find(role)?.Next?.Value;
                var roleItem = new RoleItem(
                    Id: role.Id,
                    Name: role.Name,
                    NextRoleName: nextRole?.Name ?? string.Empty
                );
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

