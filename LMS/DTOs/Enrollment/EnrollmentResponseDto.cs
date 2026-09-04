namespace LMS.DTOs.Enrollment
{
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}