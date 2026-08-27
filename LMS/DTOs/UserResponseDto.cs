namespace LMS.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImag { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
