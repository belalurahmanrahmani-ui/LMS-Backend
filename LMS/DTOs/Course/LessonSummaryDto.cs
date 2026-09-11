namespace LMS.DTOs.Course
{
    public class LessonSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int Order { get; set; }
    }
}