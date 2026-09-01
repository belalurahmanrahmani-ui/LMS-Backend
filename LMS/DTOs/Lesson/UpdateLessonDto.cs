using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Lesson
{
    public class UpdateLessonDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Url]
        public string? VideoUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int Duration { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; }
    }
}