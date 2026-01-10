using HireHuntBackend.Common.Dto;
using HireHuntBackend.Context;
using HireHuntBackend.Model;
using HireHuntBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static HireHuntBackend.Common.Enums;

namespace HireHuntBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbContextHireHunt _context;
        private readonly JwtTokenService _jwtTokenService;
        public UserController(DbContextHireHunt context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { error = "Password is required." });
            }
            if(string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return BadRequest(new { error = "FirstName is required." });
            }

            if (_context.Users.Any(x => x.Email == dto.Email))
                return BadRequest("User already exist");

            var user = new User
            {
                Email = dto.Email,
                FullName = dto.FirstName + dto.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok( new {success= dto.Email,message="User is registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { error = "Password is required." });
            }
            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var result = await _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, Roles.JobSeeker.ToString());

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            return Ok(new { result.AccessToken });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new { userId, email });
        }

    }
}
