namespace LMS.DTOs.Student
{
    public class StudentDashboardDto
    {
        public int TotalEnrolledCourses { get; set; }
        public int CompletedCourses { get; set; }
        public List<RecentLessonDto> RecentLessons { get; set; } = new();
    }
}