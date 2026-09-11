namespace LMS.DTOs.Course
{
    public class CourseFilterDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public bool? IsFree { get; set; }
    }
}