using ManageUsers.Application.Interfaces;
using ManageUsers.Domain;
using ManageUsers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AppDbContext _appDbContext;

        public AreaRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Areas.FirstOrDefaultAsync(o => o.Id == areaId, cancellationToken: cancellationToken);
        }


        public async Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Regions.FirstOrDefaultAsync(o => o.Id == regionId, cancellationToken: cancellationToken);

        }
    }
}
