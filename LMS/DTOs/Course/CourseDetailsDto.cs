namespace LMS.DTOs.Course
{
    public class CourseDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public decimal Price { get; set; }

        public string TeacherName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public int LessonCount { get; set; }
        public int TotalDuration { get; set; }
        public int EnrollmentCount { get; set; }

        public List<LessonSummaryDto> Lessons { get; set; } = new();
    }
}