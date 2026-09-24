using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        [Url(ErrorMessage = "Profile image must be a valid URL")]
        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
