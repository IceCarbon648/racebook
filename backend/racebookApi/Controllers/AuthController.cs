using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;

namespace racebookApi.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpGet("discord")]
		[Authorize]
		public IActionResult AuthenticateViaDiscord()
		{
			AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = "api/User/AmaxUsername" };

			return Challenge(properties, DiscordAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromForm] LoginDto dto)
		{
			string? jwt = await _authService.LoginAsync(dto);

			if (string.IsNullOrEmpty(jwt))
			{
				return Unauthorized();
			}

            Response.Cookies.Append("access_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new {message = "Login successful"});
		}

		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
            Response.Cookies.Delete("access_token");

            return Ok(new { message = "Logout successful" });
        }
    }
}