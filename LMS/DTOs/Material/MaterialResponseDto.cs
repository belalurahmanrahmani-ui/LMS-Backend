namespace LMS.DTOs.Material
{
    public class MaterialResponseDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}