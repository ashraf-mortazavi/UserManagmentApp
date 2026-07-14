using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAvatarAsync(IFormFile file, CancellationToken ct = default);
        Task<bool> DeleteFileAsync(string fileUrl, CancellationToken ct = default);
    }
}
