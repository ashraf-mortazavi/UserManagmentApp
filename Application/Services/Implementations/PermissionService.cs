using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Implementations
{
    public class PermissionService(IUnitOfWork unitOfWork) : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<Permission>> GetRolePermisionsByIdsAsync(List<string> permissionIds, CancellationToken cancellationToken = default)
        {
            if (permissionIds is null || permissionIds.Count == 0 || !permissionIds.Any())
            {
                return new List<Permission>();
            }
            return await _unitOfWork.Permissions.GetPermisionsByPermissionIds(permissionIds, cancellationToken);
        }
    }
}
