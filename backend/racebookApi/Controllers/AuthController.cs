using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = "api/Auth/GetAmaxUsername" };

            return Challenge(properties, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("GetAmaxUsername")]
        [Authorize]
        public async Task<IActionResult> GetAmaxUsername()
        {
            JsonDocument userAmaxData = await _authService.GetUserAmaxData(HttpContext);

            if (!_authService.HasAmaxAccount(userAmaxData))
            {
                return Ok();
            }

            string amaxUsername = _authService.GetAmaxUsername(userAmaxData);
            
            return Ok(amaxUsername);
        }


    }
}