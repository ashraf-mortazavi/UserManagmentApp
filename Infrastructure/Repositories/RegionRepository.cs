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

        public async Task<List<Zone>> GetAllZonesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Zones.ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Zone> GetZoneAsync(int regionId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Zones.FirstOrDefaultAsync(o => o.Id == regionId, cancellationToken: cancellationToken);

        }
    }
}
