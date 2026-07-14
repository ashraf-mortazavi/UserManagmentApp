using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace ManageUsers.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AppDbContext _appDbContext;

        public AreaRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Region>> GetAllZonesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Regions
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Areas.FirstOrDefaultAsync(o => o.Id == areaId, cancellationToken: cancellationToken);
        }

        public Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Area>> GetAreasByZoneAsync(int areaId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Areas.Where(r => r.Id == areaId).OrderBy(r => r.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
