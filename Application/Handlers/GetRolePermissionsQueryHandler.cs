using ManageUsers.Application.Commands;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Interfaces;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using System.Collections.Generic;

namespace ManageUsers.Application.Handlers;
public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<GetRolePermissionsResponse>>
{
    private readonly IRoleService _roleService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IPermissionService _permissionService;

    public GetRolePermissionsQueryHandler(IRoleService roleService, IRolePermissionService rolePermissionService, IPermissionService permissionService)
    {
        _roleService = roleService;
        _rolePermissionService = rolePermissionService;
        _permissionService = permissionService;
    }
    public async Task<List<GetRolePermissionsResponse>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        List<GetRolePermissionsResponse> rolePermissionsResponses = new();
        List<RolePermissionsItem> items = new();

        Role role = await _roleService.GetRoleAsync(request.RoleId, cancellationToken);

        if (role == null)
        {
            rolePermissionsResponses.Add(new GetRolePermissionsResponse()
            {
                FailedResult = "کاربر با این نقش یافت نشد!"
            });
            return rolePermissionsResponses;
        }
        List<RoleDto> roleDtos = new List<RoleDto> 
        { 
            new RoleDto (Id : role.Id, Name :role.Name!) 
        };

        var roleIds = new List<string>();
        roleIds.Add(request.RoleId);

        List<RolePermission> rolePermissions = await _rolePermissionService.GetRolePermisionsByRoleIdsAsync(roleIds, cancellationToken);
        var permissionIds = rolePermissions.Select(p => p.PermissionId.ToString()).ToList();
        List<Permission> permissions = await _permissionService.GetRolePermisionsByIdsAsync(permissionIds, cancellationToken);
        List<PermissionDto> permissionDtos = permissions
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

