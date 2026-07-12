using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IRegionService
    {
        Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default);
    }
}
