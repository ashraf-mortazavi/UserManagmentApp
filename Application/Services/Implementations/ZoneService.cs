using ManageUsers.Application.Common;
using ManageUsers.Application.Services.Interfaces;
using ManageUsers.Domain;

namespace ManageUsers.Application.Services.Implementations
{
    public class ZoneService(IUnitOfWork unitOfWork) : IZoneService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<List<Zone>> GetAllZonesAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.Zones.GetAllZonesAsync(ct);
        }

        public async Task<Zone> GetZoneAsync(int zoneId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Zones.GetZoneAsync(zoneId, cancellationToken);
        }
    }
}
