using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IZoneService
    {
        Task<List<Zone>> GetAllZonesAsync(CancellationToken ct = default);
        Task<Zone> GetZoneAsync(int zoneId, CancellationToken cancellationToken = default);
    }
}
