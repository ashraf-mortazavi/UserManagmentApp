using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IAreaRepository
    {
        Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default);
        Task<Zone> GetRegionAsync(int regionId, CancellationToken cancellationToken = default);
        Task<List<Zone>> GetAllZonesAsync(CancellationToken cancellationToken = default);

        Task<List<Area>> GetAreasByZoneAsync(int zoneIs, CancellationToken cancellationToken = default);
    }
}
