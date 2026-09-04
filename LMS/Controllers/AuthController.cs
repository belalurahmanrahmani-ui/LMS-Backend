using LMS.DTOs.Auth;
using LMS.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _aAuthService;

        public AuthController(IAuthService authService)
        {
            _aAuthService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            try
            {
                var result = await _aAuthService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _aAuthService.LoginAsyn(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {

                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var result = await _aAuthService.RefreshTokenAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException es)
            {
                return Unauthorized(new {message=es.Message});
                throw;
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _aAuthService.LogoutAsync(userId);
            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = rol
            });
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only-test")]
        public IActionResult AdminOnlyTest()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new
            {
                message = "You are an Admin Authorization works men",
                email = userEmail,
                role = userRole
            });
        }
    }
}
