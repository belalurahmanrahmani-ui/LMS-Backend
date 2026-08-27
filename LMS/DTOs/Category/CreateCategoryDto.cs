using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.CategoryDto
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }
    }
}
