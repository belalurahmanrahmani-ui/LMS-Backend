using LMS.DTOs.Progress;
using LMS.Enums;
using LMS.Services.Interface;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    [ApiController]
    public class LessonProgressController : ControllerBase
    {
        private readonly ILessonProgressService _progressService;

        public LessonProgressController(ILessonProgressService progressService)
        {
            _progressService = progressService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return int.Parse(userIdClaim);
        }

        [HttpPost("api/lessons/{lessonId}/complete")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CompleteLesson(int lessonId)
        {
            var studentId = GetCurrentUserId();
            var result = await _progressService.CompleteLessonAsync(studentId, lessonId);

            return result switch
            {
                LessonProgressOperationResult.Success => Ok(new { message = "Lesson marked as completed." }),
                LessonProgressOperationResult.LessonNotFound => NotFound(new { message = "Lesson not found." }),
                LessonProgressOperationResult.LessonNotPublished => BadRequest(new { message = "This lesson is not published yet." }),
                LessonProgressOperationResult.NotEnrolled => Forbid(),
                _ => BadRequest(new { message = "Unable to process request." })
            };
        }

        [HttpGet("api/courses/{courseId}/progress")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            var studentId = GetCurrentUserId();
            var (result, progress) = await _progressService.GetCourseProgressAsync(studentId, courseId);

            return result switch
            {
                LessonProgressOperationResult.Success => Ok(progress),
                LessonProgressOperationResult.NotEnrolled => Forbid(),
                _ => BadRequest(new { message = "Unable to process request." })
            };
        }

        [HttpGet("api/me/progress")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyProgress()
        {
            var studentId = GetCurrentUserId();
            var progress = await _progressService.GetMyProgressAsync(studentId);
            return Ok(progress);
        }
    }
}