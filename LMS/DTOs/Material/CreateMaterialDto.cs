using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Material
{
    public class CreateMaterialDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [Url]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string FileType { get; set; } = string.Empty; // مثلاً: pdf, docx, pptx, zip
    }
}