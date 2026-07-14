using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Application.Services.Implementations
{
    public class OrganizationService (IUnitOfWork unitOfWork) : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<Organization?> GetOrganizationAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Organizations.GetOrganizationAsync(organizationId, cancellationToken);
        }
    }
}
