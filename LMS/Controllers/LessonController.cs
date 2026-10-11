using LMS.DTOs.Lesson;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LMS.Enums;
using LMS.Extensions;

namespace LMS.Controllers
{

    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ICourseAccessService _courseAccessService;

        public LessonController(ILessonService lessonService, ICourseAccessService courseAccessService)
        {
            _lessonService = lessonService;
            _courseAccessService = courseAccessService;
        }
        [HttpGet("/api/courses/{courseId}/lessons")]
        [Authorize]
        public async Task<IActionResult> GetLessonByCourseId(int courseId)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var access = await _courseAccessService.GetCourseAccessAsync(courseId, userId.Value, User.GetRole());
            if (access == CourseAccess.None)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = "You do not have access to this course's lessons" });
            }

            var lessons = await _lessonService.GetLessonsByCourseAsync(courseId);

            // دانشجو فقط درس‌های منتشرشده را می‌بیند
            if (access == CourseAccess.Student)
            {
                lessons = lessons.Where(l => l.IsPublished).ToList();
            }

            return Ok(lessons);
        }

        [HttpGet("/api/lessons/{id}")]
        [Authorize]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var access = await _courseAccessService.GetLessonAccessAsync(id, userId.Value, User.GetRole());
            if (access == CourseAccess.None)
            {
                return NotFound(new { message = "Lesson Not Found" });
            }

            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null)
            {
                return NotFound(new { message = "Lesson Not Found" });
            }

            return Ok(lesson);
        }
        [HttpPost("/api/courses/{courseId}/lessons")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateLesson(int courseId, [FromBody] CreateLessonDto dto)
        {
            int teacherId = GetCurrentUserId();
            var (result, lesson) = await _lessonService.CreateLessonAsync(courseId, dto, teacherId);
            return result switch
            {
                Enums.LessonOperationResult.CourseNotFound => NotFound(new { message = "Course Not Found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => CreatedAtAction(nameof(GetLessonById), new { id = lesson!.Id }, lesson),
                _ => StatusCode(500, new { message = "An unexpected Error occurred" })
            };

        }

        [HttpPut("/api/lessons/{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonDto dto)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _lessonService.UpdateLessonAsync(id, dto, teacherId, isAdmin);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lesson not found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error accurred" })

            };
        }

        [HttpDelete("/api/lessons/{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _lessonService.DeleteLessonAsync(id, teacherId, isAdmin);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lesson not found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = " An unexpected Error accurred" })
            };
        }

        [HttpPatch("/api/lessons/{id}/publish")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> PublishLesson(int id)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _lessonService.PublishLessonAsync(id, teacherId, isAdmin);
            return result switch
            {
                Enums.LessonOperationResult.NotFound => NotFound(new { message = "Lessont Not Found" }),
                Enums.LessonOperationResult.Forbidden => Forbid(),
                Enums.LessonOperationResult.Success => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error accurred" })
            };


        }

        [HttpPatch("/api/lessons/{id}/unpublish")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UnpublishLesson(int id)
        {
            int teacherId = GetCurrentUserId();
            bool isAdmin = IsCurrentUserAdmin();
            var result = await _lessonService.UnpublishLessonAsync(id, teacherId, isAdmin);
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

        private bool IsCurrentUserAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}