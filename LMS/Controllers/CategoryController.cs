using LMS.DTOs.CategoryDto;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var category = await _categoryService.GetAllCategoryAsync();
            if (category == null)
                return NotFound(new { message = " Category Not Found" });
            return Ok(category);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var category = await _categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (InvalidOperationException ex)

            {

                return Conflict(new { message = ex.Message });
            }

        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new { message = "Category not found" });
                return Ok(category);
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory (int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, dto);
                if (category == null)
                    return NotFound(new { message = "Category not found" });
                return Ok(category);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new {message=ex.Message});
                
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoy(int id)
        {
            try
            {
                var deleted = await _categoryService.DeletCategoryAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Category not found" });
                return NoContent();
            }
            catch (DbUpdateException)
            {

                return Conflict(new { message = "conot this category because courses are linked to in" });
            }
        }
    }
}
