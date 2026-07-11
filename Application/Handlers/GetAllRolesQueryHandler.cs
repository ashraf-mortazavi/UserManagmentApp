using ManageUsers.Application.DTOs;
using ManageUsers.Application.Queries;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Handlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, GetAllRolesResponse>
    {
        private readonly IRoleService _roleService;

        public GetAllRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetAllRolesResponse> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            List<Role> roles = await _roleService.GetAllRolesAsync(cancellationToken);

            GetAllRolesResponse response = new()
            {
                Roles = roles.Select(r => new RoleResponse
                {
                    Id = r.Id.ToString(),
                    Name = r.Name ?? string.Empty
                }).ToList()
            };

            return response;
        }
    }
}
