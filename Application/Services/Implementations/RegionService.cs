using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Implementations
{
    public class RegionService(IUnitOfWork unitOfWork) : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        

        public async Task<Region> GetRegionAsync(int regionId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Regions.GetZoneAsync(regionId, cancellationToken);
        }
    }
}
