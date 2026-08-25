using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Auth
{
    public class LoginDto
    {
        public string FullName { get; internal set; }= string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }= string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }= string.Empty;
    }
}
