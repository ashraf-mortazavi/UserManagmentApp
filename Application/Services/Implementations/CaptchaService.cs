using ManageUsers.Application.DTOs;
using ManageUsers.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using SkiaSharp;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ManageUsers.Application.Services.Implementations
{
    public class CaptchaService : ICaptchaService
    {
        private readonly IDistributedCache _cache;
        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(2);
        private static string Key(string id) => $"captcha:{id}";
        const string digits = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

        public CaptchaService(IDistributedCache cache) => _cache = cache;


        public async Task<GetCaptchaResponse> GenerateCaptchaImageAsync(CancellationToken ct)
        {
            string text = GenerateRandomText(5);
            string captchaId = Guid.NewGuid().ToString("N");
            string hash = ComputeSha256Hash(text);

            await _cache.SetStringAsync(
                Key(captchaId),
                hash,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Expiration },
            ct);

            byte[] imageBytes = GenerateImage(text);
            return new GetCaptchaResponse { 
                CaptchaId = captchaId,
                CaptchaCode =  Convert.ToBase64String(imageBytes)};
        }

        public async Task<bool> ValidateCaptchaAsync(string captchaId, string userInput, CancellationToken ct)
        {
      
            if (string.IsNullOrWhiteSpace(captchaId) || string.IsNullOrWhiteSpace(userInput))
                return false;
            userInput = userInput.Trim();

            string? storedHash = await _cache.GetStringAsync(Key(captchaId), ct);
            if (storedHash is null)
                return false;

            await _cache.RemoveAsync(Key(captchaId), ct); // one-time use

            string userInputHash = ComputeSha256Hash(userInput);
            return string.Equals(storedHash, userInputHash, StringComparison.OrdinalIgnoreCase  );
        }

        private static string ComputeSha256Hash(string rawData)
        {
            rawData = rawData.Trim().ToUpperInvariant();
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        private static string GenerateRandomText(int length)
        {
            return new string(Enumerable.Range(0, length)
                .Select(_ => digits[RandomNumberGenerator.GetInt32(digits.Length)])
                .ToArray());
        }

        public static byte[] GenerateImage(string text)
        {
            using SKBitmap bitmap = new(100, 40);
            using SKCanvas canvas = new(bitmap);
            canvas.Clear(SKColors.White);

            using SKPaint paint = new()
            {
                Color = SKColors.Black,
                IsAntialias = true,
            };

            using SKFont font = new(SKTypeface.FromFamilyName("Arial"), size: 22);
            using SKTextBlob textBlob = SKTextBlob.Create(text, font);

            if (textBlob is not null)
            {
                canvas.DrawText(textBlob, 10, 28, paint);
            }
            using SKImage image = SKImage.FromBitmap(bitmap);
            Console.WriteLine(image);
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}