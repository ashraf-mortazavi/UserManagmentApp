using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Queries;
using ManageUsers.Domain;
using MediatR;
using System.Collections.Generic;

namespace ManageUsers.Application.Handlers;
public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<GetRolePermissionsResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetRolePermissionsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<List<GetRolePermissionsResponse>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        List<GetRolePermissionsResponse> rolePermissionsResponses = new();
        List<RolePermissionsItem> items = new();

        List<Role> roles = await _userRepository.GetRolesAsync(request.RoleIds, cancellationToken);

        if (roles == null || !roles.Any())
        {
            rolePermissionsResponses.Add(new GetRolePermissionsResponse()
            {
                FailedResult = "کاربر یافت نشد!"
            });
            return rolePermissionsResponses;
        }
        List<RoleDto> roleDtos = roles.Select(r => new RoleDto (Id: r.Id, Name: r.Name)).ToList();

        List<RolePermission> rolePermissions = await _userRepository.GetRolePermisionsByRoleIds(request.RoleIds, cancellationToken);

        List<PermissionDto> permissionDtos = rolePermissions
            .Select(rp => rp.Permission)
            .Where(p => p is not null)
            .Select(p => new PermissionDto(p!.Id, p!.Key, p!.Name, p!.IsActive, p!.SortOrder, p!.ParentId))
            .OrderBy(p => p.SortOrder)
            .ToList();

        items.Add(new RolePermissionsItem(Roles: roleDtos, Permissions: permissionDtos));

        rolePermissionsResponses.AddRange(new GetRolePermissionsResponse()
        {
             Items = items
        });

        return rolePermissionsResponses;
    }
}

