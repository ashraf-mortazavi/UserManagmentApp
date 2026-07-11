using ManageUsers.Application.DTOs;

namespace ManageUsers.Application.Services.Interfaces
{
    public interface ICaptchaService
    {
        Task<GetCaptchaResponse> GenerateCaptchaImageAsync(CancellationToken ct);
        Task<bool> ValidateCaptchaAsync(string captchaId, string userInput, CancellationToken ct);
    }
}
