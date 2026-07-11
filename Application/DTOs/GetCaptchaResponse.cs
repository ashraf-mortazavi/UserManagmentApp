namespace ManageUsers.Application.DTOs
{
    public class GetCaptchaResponse
    {
        public string CaptchaId { get; set; } = string.Empty;
        public string CaptchaCode { get; set; } = string.Empty;
    }
}
