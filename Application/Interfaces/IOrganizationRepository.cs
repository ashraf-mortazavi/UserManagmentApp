using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<Organization> GetOrganizationAsync(int organizationId, CancellationToken cancellationToken = default);
    }
}
