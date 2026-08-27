using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.CategoryDto
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }
    }
}
