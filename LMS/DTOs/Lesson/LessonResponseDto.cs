namespace LMS.DTOs.Lesson
{
    public class LessonResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
        public bool IsPublished { get; set; }
    }
}