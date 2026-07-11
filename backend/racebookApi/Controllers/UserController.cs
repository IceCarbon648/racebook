using AmaxApiAdapter.Adapters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Models.DTOs.Request;
using Business.Interfaces;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAmaxAdapter _amaxAdapter;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAmaxAdapter amaxAdapter, IAuthService authService)
        {
            _userService = userService;
            _amaxAdapter = amaxAdapter;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUserDto dto)
        {
            bool userIsRegistered = await _userService.RegisterUserAsync(dto);

            return Ok(new { message = "Registration successful" });
        }

        [HttpGet("callback")]
        [Authorize]
        public async Task<IActionResult> SetAmaxUsername()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            string? amaxUsername = await _amaxAdapter.GetAmaxUsername(await HttpContext.GetTokenAsync("access_token"));

            if (string.IsNullOrEmpty(amaxUsername) || string.IsNullOrEmpty(uid))
            {
                return Ok(new { message = "No amax account associated with the discord account" });
            }

            string? username = User.FindFirst(ClaimTypes.Name)?.Value.ToString();
            await _userService.setAmaxUsername(uid, amaxUsername);
            Response.Cookies.Delete("access_token");

            string jwt = _authService.GenerateTokenWithAmaxUsername(uid, username, amaxUsername);

            Response.Cookies.Append("access_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new { message = "Saved amax username" });
        }
    }
}