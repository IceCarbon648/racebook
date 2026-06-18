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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserDto([FromForm] RegisterUserDto dto)
        {
            bool userIsRegistered = await _userService.RegisterUserAsync(dto);

            return Ok();
        }
    }
}