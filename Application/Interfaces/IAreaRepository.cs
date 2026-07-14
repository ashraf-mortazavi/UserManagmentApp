using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IAreaRepository
    {
        Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default);
        Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default);
        Task<List<Region>> GetAllZonesAsync(CancellationToken cancellationToken = default);

        Task<List<Area>> GetAreasByZoneAsync(int areaId, CancellationToken cancellationToken = default);
    }
}
