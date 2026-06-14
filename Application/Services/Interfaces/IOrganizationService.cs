using ManageUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> GetOrganizationAsync(int organizationId, CancellationToken cancellationToken = default);
    }
}
