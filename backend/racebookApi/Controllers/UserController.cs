using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;
using AmaxApiAdapter.Adapters;

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

        [HttpGet("amax-stats")]
        public async Task<IActionResult> GetPlayerStats()
        {
            return Ok(await _amaxAdapter.GetPlayerStats("IceCarbon"));
        }
    }
}