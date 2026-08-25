namespace LMS.Helpers
{
        // This  class mirrors the "JwtSettngs" section in appsetting.json
        // propery names must match the JSON keys exactly (case-insnsitive binding).
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set;  } = string.Empty;
        public string Audience { get; set; } =string .Empty;
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}
