using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage ="Refresh token is required")]
        public string RefreshToken { get; set; }=string.Empty;

    }
}
