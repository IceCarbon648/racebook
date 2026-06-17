using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;
using System.Text.Json;

namespace racebookApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpGet("AuthenticateViaDiscord")]
		public IActionResult AuthenticateViaDiscord()
		{
			AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = "api/Auth/AmaxUsername" };

			return Challenge(properties, DiscordAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpPost("AmaxUsername")]
		[Authorize]
		public async Task<IActionResult> SetAmaxUsername()
		{
			JsonDocument userAmaxData = await _authService.GetUserAmaxData(HttpContext);

			if (!_authService.HasAmaxAccount(userAmaxData))
			{
				return Ok("No amax account associated with the discord account");
			}

			string amaxUsername = _authService.GetAmaxUsername(userAmaxData);
			await _authService.setAmaxUsername(amaxUsername);

			return Ok("set amax name");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromForm] LoginDto dto)
		{
			string? jwt = await _authService.LoginAsync(dto);

			if (string.IsNullOrEmpty(jwt))
			{
				return Unauthorized();
			}

			return Ok(jwt);
		}
	}
}