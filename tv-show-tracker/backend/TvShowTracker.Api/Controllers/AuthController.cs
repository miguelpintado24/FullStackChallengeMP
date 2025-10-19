using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using TvShowTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TvShowTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Username, request.Password);
                return Ok(new { user.Id, user.Username });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var username = User.Identity!.Name!;
                await _authService.DeleteAccountAsync(username);
                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // DTOs
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
