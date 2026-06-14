using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class OrganizationRepository(AppDbContext appDbContext) : IOrganizationRepository
    {
        private readonly AppDbContext _appDbContext;

        public async Task<Organization> GetOrganizationAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken: cancellationToken);
        }
    }
}
