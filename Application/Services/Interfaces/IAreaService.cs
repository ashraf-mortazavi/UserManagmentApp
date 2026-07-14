using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IAreaService
    {
        Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default);

        Task<List<Area>> GetAreasByZone(int areaId, CancellationToken ct = default);
    }
}
