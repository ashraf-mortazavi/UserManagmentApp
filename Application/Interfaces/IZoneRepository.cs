using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IZoneRepository
    {
        Task<Zone> GetZoneAsync(int regionId, CancellationToken cancellationToken = default);
        Task<List<Zone>> GetAllZonesAsync(CancellationToken cancellationToken = default);
    }
}
