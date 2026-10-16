using System.ComponentModel.DataAnnotations;
using LMS.Enums;

namespace LMS.DTOs
{
    public class AdminCreateUserDto
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
    }
}