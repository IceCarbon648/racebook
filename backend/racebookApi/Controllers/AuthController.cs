using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Request;
using Models.Validators.Filter;

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

		[HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilter<LoginDto>))]
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

            return Ok(new { message = "Login successful" });
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