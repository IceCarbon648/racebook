using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Request;
using Models.Validators.Filter;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilter<RegisterUserDto>))]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUserDto dto)
        {
            if (!await _userService.RegisterUserAsync(dto))
            {
                return Conflict(new { message = "Registration was not successful" });
            }

            return Ok(new { message = "Registration successful" });
        }

        [HttpGet("@me")]
        [Authorize]
        public async Task<IActionResult> GetTokenClaims()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            string? amaxUsername = User.FindFirst(ClaimTypes.GivenName)?.Value;

            return Ok(new { uid, username, amaxUsername });
        }
    }
}