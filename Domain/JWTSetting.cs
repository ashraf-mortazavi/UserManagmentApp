namespace ManageUsers.Domain
{
    public class JWTSetting
    {
        public string SecretKey { get; set; } = string.Empty;   // min 32 bytes for HS256
        public string EncryptKey { get; set; } = string.Empty;  // 16 bytes for AES128
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double TokenExpirationInHours { get; set; } = 3;
        public int RefreshTokenExpirationInDays { get; set; } = 7;
        public double NotBeforeMinutes { get; set; } = 0;       // usually 0 means off
    }
}