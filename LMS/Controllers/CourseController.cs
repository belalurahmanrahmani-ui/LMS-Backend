using LMS.DTOs.Course;
using LMS.Enums;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService; 
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourse()
        {
            var course = await _courseService.GetAllCoursesAsync();
            return Ok(course);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found" });
            return Ok(course);
        }
        [HttpGet("my-course")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> GetMyCourse()
        {
            int teacherId = GetCurrentUserId();
            var course = await _courseService.GetMyCoursesAsync(teacherId);
            return Ok(course); 
        }
        [HttpPost]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            int teacherId = GetCurrentUserId();
            var (result, course) = await _courseService.CreateCourseAsync(dto, teacherId);
            if (result == CourseOperationResult.InvalidCategor)
                return BadRequest(new { message = "invalid category id" });
            return CreatedAtAction(nameof(GetCourseById), new {id = course!.Id},course);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            int teacherId = GetCurrentUserId();
            var result = await _courseService.DeleteCourseAsync(id, teacherId);

            return result switch
            {
                CourseOperationResult.NotFound => NotFound(new { message = "Course not found" }),
                CourseOperationResult.Forbidden => Forbid(),
                CourseOperationResult.Sucses => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error occurred." })
            };
        }
        [HttpPut("{id}")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto) {

            int teacherId = GetCurrentUserId();
            var result = await _courseService.UpdateCourseAsync(id, dto, teacherId);

            return result switch
            {
                CourseOperationResult.NotFound => NotFound(new { message = "Course not found" }),
                CourseOperationResult.Forbidden =>Forbid(),
                CourseOperationResult.InvalidCategor => BadRequest(new {message="invlid category"}),
                CourseOperationResult.Sucses => NoContent(),
                _ => StatusCode(500,new {message = "an unexpected error ocured"})
            };

        }

        [HttpPatch("{id}/publish")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> PublishCourse(int id)
        {
            int teacherId = GetCurrentUserId();
            var result =  await _courseService.PublishCourseAsync(id,teacherId);
            
            return result switch
            {
                CourseOperationResult.NotFound => NotFound(new { message = "Course not found" }),
                CourseOperationResult.Forbidden => Forbid(),
                CourseOperationResult.InvalidCategor => BadRequest(new { message = "invlid category" }),
                CourseOperationResult.Sucses => NoContent(),
                _ => StatusCode(500, new { message = "an unexpected error ocured" })
            };
        }
        [HttpPatch("{id}/unpublish")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UnpublishCourse(int id)
        {
            int teacherId = GetCurrentUserId();
            var result = await _courseService.UnpublishCourseAsync(id, teacherId);

            return result switch
            {
                CourseOperationResult.NotFound => NotFound(new { message = "Course not found" }),
                CourseOperationResult.Forbidden => Forbid(),
                CourseOperationResult.Sucses => NoContent(),
                _ => StatusCode(500, new { message = "An unexpected error occurred." })
            };
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim!);
        }
    }
}
