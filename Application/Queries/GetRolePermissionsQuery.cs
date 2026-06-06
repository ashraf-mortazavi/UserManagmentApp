using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public class GetRolePermissionsQuery(List<int> roleIds) : IRequest<List<GetRolePermissionsResponse>>
    {
        public List<int> RoleIds { get; set; } = roleIds;

    }
}
