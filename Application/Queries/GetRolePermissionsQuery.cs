using ManageUsers.Application.DTOs;
using MediatR;

namespace ManageUsers.Application.Queries
{
    public class GetRolePermissionsQuery(string roleId) : IRequest<List<GetRolePermissionsResponse>>
    {
        public string RoleId { get; set; } = roleId;

    }
}
