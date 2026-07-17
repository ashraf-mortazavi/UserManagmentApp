using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Implementations
{
    public class AreaService(IUnitOfWork unitOfWork) : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<Area> GetAreaAsync(int areaId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Areas.GetAreaAsync(areaId, cancellationToken);
        }

        public async Task<List<Area>> GetAreasByZone(int zoneId, CancellationToken ct = default)
        {
            return await _unitOfWork.Areas.GetAreasByZoneAsync(zoneId, ct);
        }
    }
}
