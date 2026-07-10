namespace ManageUsers.Application.Services.Interfaces
{
    public interface ICaptchaService
    {
        Task<(string CaptchaId, string ImageBase64)> GenerateCaptchaImageAsync(CancellationToken ct);
        Task<bool> ValidateCaptchaAsync(string captchaId, string userInput, CancellationToken ct);
    }
}
