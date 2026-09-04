using LMS.DTOs.Lesson;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService; 
        }
        [HttpGet("/api/courses/{courseId}/lessons")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessonByCourseId(int courseId)
        {
            var lesson = await _lessonService.GetLessonsByCourseAsync(courseId);
            return Ok(lesson);
        }
        [HttpGet("/api/lessons/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null)
                return NotFound(new { message = "Lesson Not Found" });
            return Ok(lesson);
        }

        [HttpPost("/api/courses/{courseId}/lessons")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateLesson(int courseId, [FromBody] CreateLessonDto dto)
        {
            int teacherId = GetCurrentUserId();
            var (result , lesson) = await _lessonService.CreateLessonAsync(courseId, dto,teacherId) ;
            return result switch
            {
                Enums.LessonOperationResult.CourseNotFound => NotFound(new { message = "Course Not Found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => CreatedAtAction(nameof(GetLessonById), new { id = lesson!.Id }, lesson),
                _ => StatusCode(500, new { message = "An unexpected Error occurred" })
            };

        }

        [HttpPut("/api/lessons/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonDto dto)
        {
            int teacherId = GetCurrentUserId();
            var result = await _lessonService.UpdateLessonAsync(id, dto, teacherId);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lesson not found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error accurred" })

            };
        }

        [HttpDelete("/api/lessons/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            int teacherId = GetCurrentUserId();
            var result = await _lessonService.DeleteLessonAsync(id, teacherId);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lesson not found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = " An unexpected Error accurred" })
            };
        }

        [HttpPatch("/api/lessons/{id}/publish")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PublishLesson(int id)
        {
            int teacherId = GetCurrentUserId();
            var result = await _lessonService.PublishLessonAsync(id, teacherId);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lessont Not Found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error accurred" })
            };


        }

        [HttpPatch("/api/lessons/{id}/unpublish")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> UnpublishLesson (int id)
        {
            int teacherId = GetCurrentUserId();
            var result = await _lessonService.UnpublishLessonAsync(id, teacherId);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lesson Not Found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected Error accurred" })
            };
        }
        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim!);
        }
    }
}

