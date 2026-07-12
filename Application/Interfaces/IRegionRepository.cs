using ManageUsers.Domain;

namespace ManageUsers.Application.Interfaces
{
    public interface IRegionRepository
    {
        Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default);
    }
}
