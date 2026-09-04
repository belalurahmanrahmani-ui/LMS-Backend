using LMS.Entities;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentController(IEnrollmentService service)
        {
            _enrollmentService = service;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return int.Parse(userIdClaim);
        }
        [HttpPost("api/courses/{courseId}/enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var studentId = GetCurrentUserId();
            var (result, enrollment) = await _enrollmentService.EnrollStudentAsync(studentId, courseId);
            return result switch
            {
                EnrollmentOperationResult.Success => Ok(enrollment),
                EnrollmentOperationResult.CourseNotFound => NotFound(new { message = "Course not found." }),
                EnrollmentOperationResult.CourseNotPublished => BadRequest(new { message = "This course is not published yet." }),
                EnrollmentOperationResult.AlreadyEnrolled => Conflict(new { message = "You are already enrolled in this course." }),
                _ => BadRequest(new { message = "Unable to process enrollment." })
            };
        }
        [HttpGet("api/me/courses")]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> GetMyCourse()
        {
            var studentId = GetCurrentUserId();
            var course = await _enrollmentService.GetMyCourseAsyn(studentId);
            return Ok(course);
        }
    }
}
