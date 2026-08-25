namespace LMS.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        public string AccessToken { get; set; }= string.Empty;
        public string RefreshToken { get; set; }= string.Empty;
        public DateTime AccessTokenExpiration { get; set; }

        // Not : PasswordHash is intentionally NOT here.
        // We never send password-related information back to the client for security reasons.


    }
}
