using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly AppDbContext _appDbContext;

        public RegionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Regions.FirstOrDefaultAsync(o => o.Id == regionId, cancellationToken: cancellationToken);

        }
    }
}
