using LMS.DTOs.Material;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LMS.Enums;
using LMS.Extensions;

namespace LMS.Controllers
{
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly ICourseAccessService _courseAccessService;
        public MaterialController(IMaterialService materialService , ICourseAccessService courseAccessService)
        {
            _materialService = materialService;
            _courseAccessService = courseAccessService;
        }

        [HttpGet("/api/lessons/{lessonId}/materials")]
        [Authorize]
        public async Task<IActionResult> GetMaterialsByLesson(int lessonId)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var access = await _courseAccessService.GetLessonAccessAsync(lessonId, userId.Value, User.GetRole());
            if (access == CourseAccess.None)
            {
                return NotFound(new { message = "Lesson Not Found" });
            }

            var materials = await _materialService.GetMaterialsByLessonAsync(lessonId);
            return Ok(materials);
        }

        [HttpGet("/api/materials/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMaterialById(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound(new { message = "Material not found" });
            }

            var access = await _courseAccessService.GetLessonAccessAsync(material.LessonId, userId.Value, User.GetRole());
            if (access == CourseAccess.None)
            {
                return NotFound(new { message = "Material not found" });
            }

            return Ok(material);
        }

        [HttpPost("/api/lessons/{lessonId}/materials")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateMaterial(int lessonId, [FromBody] CreateMaterialDto dto)
        {
            int teacherId = GetCurrentUserId();
            var (result, material) = await _materialService.CreateMaterialAsync(lessonId, dto, teacherId);

            return result switch
            {
                MaterialOperationResult.LessonNotFound => NotFound(new { message = "Lesson not found" }),
                MaterialOperationResult.Forbidden => Forbid(),
                MaterialOperationResult.Success => CreatedAtAction(nameof(GetMaterialById), new { id = material!.Id }, material),
                _ => StatusCode(500, new { message = "An unexpected error occurred." })
            };
        }

        [HttpPut("/api/materials/{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] UpdateMaterialDto dto)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _materialService.UpdateMaterialAsync(id, dto, teacherId, isAdmin);

            return result switch
            {
                MaterialOperationResult.NotFound => NotFound(new { message = "Material not found" }),
                MaterialOperationResult.Forbidden => Forbid(),
                MaterialOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error occurred." })
            };
        }

        [HttpDelete("/api/materials/{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _materialService.DeleteMaterialAsync(id, teacherId, isAdmin);

            return result switch
            {
                MaterialOperationResult.NotFound => NotFound(new { message = "Material not found" }),
                MaterialOperationResult.Forbidden => Forbid(),
                MaterialOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error occurred." })
            };
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim!);
        }

        private bool IsCurrentUserAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}