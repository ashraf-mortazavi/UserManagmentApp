using ManageUsers.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAvatarAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            throw new InvalidOperationException("فایل آپلود شده معتبر نیست.");

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("حجم فایل نباید بیشتر از ۵ مگابایت باشد.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("فرمت فایل مجاز نیست. فقط فرمت های می توانند آپلود شوند! jpg، jpeg، png و webp .");

        var uploadsDir = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return $"/uploads/avatars/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string fileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.FromResult(false);

        var webRoot = _env.WebRootPath ?? _env.ContentRootPath;
        var fullPath = Path.Combine(webRoot, fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
