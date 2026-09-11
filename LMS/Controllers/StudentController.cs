using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentDashboardService _studentDashboardService;

        public StudentController(IStudentDashboardService studentDashboardService)
        {
            _studentDashboardService = studentDashboardService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetStudentDashboard()
        {
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _studentDashboardService.GetStudentDashboardAsync(studentId);
            return Ok(result);
        }
    }
}