using AmaxApiAdapter.Adapters;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services;
using racebookApi.Services.Interfaces;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAmaxAdapter _amaxAdapter;

        public UserController(IUserService userService, IAmaxAdapter amaxAdapter)
        {
            _userService = userService;
            _amaxAdapter = amaxAdapter;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserDto([FromForm] RegisterUserDto dto)
        {
            bool userIsRegistered = await _userService.RegisterUserAsync(dto);

            return Ok();
        }

        [HttpGet("amaxUsername")]
        [Authorize]
        public async Task<IActionResult> SetAmaxUsername()
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string amaxUsername = await _amaxAdapter.GetAmaxUsername(await HttpContext.GetTokenAsync("access_token"));

            if (!string.IsNullOrEmpty(amaxUsername))
            {
                await _userService.setAmaxUsername(uid, amaxUsername);

                return Ok($"set amax name\n{uid}");
            }

            return Ok("No amax account associated with the discord account");
        }
    }
}