using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
    }
}
