using LMS.DTOs;
using LMS.DTOs.Admin;
using LMS.Helpers;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdminDashboardService _adminService;
        private readonly int _primaryAdminId;

        public AdminController(
            IUserService userService,
            IAdminDashboardService adminDashboard,
            IOptions<AdminSettings> adminSettings)
        {
            _userService = userService;
            _adminService = adminDashboard;
            _primaryAdminId = adminSettings.Value.PrimaryAdminId;
        }

        // ---------- Helpers ----------
        private int CurrentUserId =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // اگر مجاز نیست پاسخ خطا برمی‌گرداند، اگر مجاز است null
        private async Task<IActionResult?> EnsureCanManageAsync(int targetId)
        {
            var targetRole = await _userService.GetUserRoleAsync(targetId);
            if (targetRole == null)
                return NotFound(new { message = "User not found." });

            if (!UserManagementPolicy.CanManage(
                    CurrentUserId, targetId, targetRole.Value, _primaryAdminId))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You do not have permission to manage this user." });

            return null;
        }

        // ---------- Endpoints ----------
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUserAsync();
            foreach (var u in users)
                u.IsPrimaryAdmin = u.Id == _primaryAdminId;
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });
            user.IsPrimaryAdmin = user.Id == _primaryAdminId;
            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserManagementPolicy.CanAssignRole(CurrentUserId, dto.Role, _primaryAdminId))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Only the primary admin can create another admin." });

            try
            {
                var created = await _userService.CreateUserAsync(dto);
                return Created($"/api/Admin/users/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ویرایش پروفایل خودت مجاز است، برای بقیه Policy چک می‌شود
            if (id != CurrentUserId)
            {
                var denied = await EnsureCanManageAsync(id);
                if (denied != null) return denied;
            }

            try
            {
                var updated = await _userService.UpdateUserAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = "User not found." });
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            if (CurrentUserId == id)
                return BadRequest(new { message = "You cannot delete your own account." });

            var denied = await EnsureCanManageAsync(id);
            if (denied != null) return denied;

            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                    return NotFound(new { message = "User not found." });
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Cannot delete this user because related data exists (e.g. courses or enrollments)."
                });
            }
        }

        [HttpPatch("users/{id}/active")]
        public async Task<IActionResult> ActiveUser(int id)
        {
            var denied = await EnsureCanManageAsync(id);
            if (denied != null) return denied;

            var success = await _userService.SetActiveStatusAsync(id, true);
            if (!success)
                return NotFound(new { message = "User not found." });
            return Ok(new { message = "User activated successfully." });
        }

        [HttpPatch("users/{id}/deactive")]
        public async Task<IActionResult> DeactiveUser(int id)
        {
            if (CurrentUserId == id)
                return BadRequest(new { message = "You cannot deactivate yourself." });

            var denied = await EnsureCanManageAsync(id);
            if (denied != null) return denied;

            var success = await _userService.SetActiveStatusAsync(id, false);
            if (!success)
                return NotFound(new { message = "User not found." });
            return Ok(new { message = "User deactivated successfully." });
        }

        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (CurrentUserId == id)
                return BadRequest(new { message = "You cannot change your own role." });

            var denied = await EnsureCanManageAsync(id);
            if (denied != null) return denied;

            if (!UserManagementPolicy.CanAssignRole(CurrentUserId, dto.Role, _primaryAdminId))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Only the primary admin can assign the admin role." });

            var success = await _userService.ChangeRoleAsync(id, dto.Role);
            if (!success)
                return NotFound(new { message = "User not found." });
            return Ok(new { message = $"User role changed to {dto.Role} successfully." });
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _adminService.GetAdminDashboardAsync();
            return Ok(result);
        }
    }
}