using LMS.DTOs.CategoryDto;
using LMS.Entities;

namespace LMS.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllCategoryAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto?> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto> UpdateCategoryAsync(int id,UpdateCategoryDto dto);
        Task<bool> DeletCategoryAsync(int id);
    }
}
