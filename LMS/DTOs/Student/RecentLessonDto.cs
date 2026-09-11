namespace LMS.DTOs.Student
{
    public class RecentLessonDto
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }
}