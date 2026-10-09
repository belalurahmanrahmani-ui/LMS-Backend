using System.Security.Claims;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [ApiController]
    [Authorize(Roles = "Teacher")]
    [Route("api/courses/{courseId}/students")]
    public class CourseStudentsController : ControllerBase
    {
        private readonly ILessonProgressService _lessonProgressService;

        public CourseStudentsController(ILessonProgressService lessonProgressService)
        {
            _lessonProgressService = lessonProgressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized();
            }

            var students = await _lessonProgressService.GetCourseStudentsAsync(courseId, teacherId);
            if (students == null)
            {
                return NotFound(new { message = "Course not found" });
            }

            return Ok(students);
        }
    }
}