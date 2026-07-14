using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class RegionRepository : IZoneRepository
    {
        private readonly AppDbContext _appDbContext;

        public RegionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Region>> GetAllZonesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Regions.ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Region> GetZoneAsync(int regionId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Regions.FirstOrDefaultAsync(o => o.Id == regionId, cancellationToken: cancellationToken);

        }
    }
}
