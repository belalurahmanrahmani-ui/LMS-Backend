using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherDashboardService _teacherDashboardService;

        public TeacherController(ITeacherDashboardService teacherDashboardService)
        {
            _teacherDashboardService = teacherDashboardService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetTeacherDashboard()
        {
            var teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _teacherDashboardService.GetTeacherDashboardAsync(teacherId);
            return Ok(result);
        }
    }
}