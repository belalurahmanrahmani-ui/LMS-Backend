using System.Security.Claims;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [ApiController]
    [Authorize(Roles = "Student")]
    [Route("api/courses/{courseId}/completed-lessons")]
    public class CompletedLessonsController : ControllerBase
    {
        private readonly ILessonProgressService _lessonProgressService;

        public CompletedLessonsController(ILessonProgressService lessonProgressService)
        {
            _lessonProgressService = lessonProgressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedLessons(int courseId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var studentId))
            {
                return Unauthorized();
            }

            var lessonIds = await _lessonProgressService.GetCompletedLessonIdsAsync(studentId, courseId);
            if (lessonIds == null)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = "You are not enrolled in this course" });
            }

            return Ok(lessonIds);
        }
    }
}