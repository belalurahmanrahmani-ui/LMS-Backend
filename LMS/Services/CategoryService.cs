using LMS.Data;
using LMS.DTOs.CategoryDto;
using LMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly LmsDbContext _context;
        public CategoryService(LmsDbContext context)
        {
            _context = context; 
        }
        public async Task<CategoryResponseDto?> CreateCategoryAsync(CreateCategoryDto dto)
        {
            
            bool nameTaken = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
            if (nameTaken)
                throw new InvalidOperationException("A category with this name is already exists.");
            var category = new Category
            {
                Name = dto.Name,
                Descriptions = dto.Description
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Descriptions
            };
        }

        public async Task<bool> DeletCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoryAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Descriptions
                }).ToListAsync();
            
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Descriptions
                }).FirstOrDefaultAsync();
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;
            bool nameTaken = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);
            if(nameTaken)
            {
                throw new InvalidOperationException("a category with this name is already exists.");
                
            }
            category.Name = dto.Name;
            category.Descriptions = dto.Description;
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Descriptions
            };
        }
    }
}
