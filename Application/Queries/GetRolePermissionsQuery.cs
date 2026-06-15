using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public class GetRolePermissionsQuery(List<string> roleIds) : IRequest<List<GetRolePermissionsResponse>>
    {
        public List<string> RoleIds { get; set; } = roleIds;

    }
}
