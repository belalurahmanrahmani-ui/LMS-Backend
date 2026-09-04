using LMS.DTOs;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService; 
        }

        [HttpPut("users{id}")]
        public async Task <IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updateUser = await _userService.UpdateUserAsync(id, dto);
                if(updateUser == null)
                    return NotFound(new { message = "User not found." });
                return Ok(updateUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }

        }

        /// this is just for testing purpose, in real application we will have more admin functionalities like managing courses, lessons, categories, etc.
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUserAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { massege = "User not found." });
            return Ok(user);

        }
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
             var currentUserId = int.Parse(User
                 .FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId == id)
                return BadRequest(new { message = "You canot delete your own acount bro ." });
            try
            {
                var deleteUser = await _userService.DeleteUserAsync(id);
                if (!deleteUser)
                {
                    return NotFound(new { massage = "User Is not found" });
                }
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(
                    new { massage = "canot delete thsi user becuse related data is exist (e.g. course or enrollment " });

            }
        }
        [HttpPatch("users/{id}/active")]
        public async Task<IActionResult> ActiveUser(int id)
        {
            var success = await _userService.SetActiveStatusAsync(id, true);
            if (!success)
                return NotFound(new { message = "User Not Found." });
            return Ok(new { message = "User activated successfully." });
        }
        [HttpPatch("users/{id}/deactive")]
        public async Task<IActionResult> DeactiveUser(int id)
        {
            var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if(currentUser == id)
            {
                return BadRequest(new { message = "You canot deactive your self bro ." });
                
            }
            var success = await _userService.SetActiveStatusAsync(id, false);
            if (!success)
                return NotFound(new { message = "User not found" });
            return Ok(new { message = "User deactivated successfully." });
        }
        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId == id)
                return BadRequest(new { message = "You conot change your own role" });
            var success = await _userService.ChangeRoleAsync(id, dto.Role);
            if (!success)
                return NotFound(new { message = "User not found" });
            return Ok(new { message = $"User role changed to {dto.Role} successfully ." });
        }

    }
}
