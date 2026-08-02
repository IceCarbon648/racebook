using Business.Interfaces;
using Models.DTOs.Request;
using Models.Validators.Filter;
using Microsoft.AspNetCore.Mvc;

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
                return Unauthorized(new { message = "Registration was not successful" });
            }

            return Ok(new { message = "Registration successful" });
        }
    }
}