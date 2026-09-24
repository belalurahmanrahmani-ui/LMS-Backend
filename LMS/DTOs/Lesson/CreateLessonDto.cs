using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Lesson
{
    public class CreateLessonDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Url]
        public string? VideoUrl { get; set; }

        
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes.")]
        public int Duration { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Order cannot be negative.")]
        public int Order { get; set; }
    }
}