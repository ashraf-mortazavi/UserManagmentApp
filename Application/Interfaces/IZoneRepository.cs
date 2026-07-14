using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IZoneRepository
    {
        Task<Region> GetZoneAsync(int regionId, CancellationToken cancellationToken = default);
        Task<List<Region>> GetAllZonesAsync(CancellationToken cancellationToken = default);
    }
}
