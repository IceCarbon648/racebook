using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/favourite-mod")]
    public class FavouriteModController : Controller
    {
        private readonly IFavouriteModRepository _favouriteModRepository;

        public FavouriteModController(IFavouriteModRepository favouriteModRepository)
        {
            _favouriteModRepository = favouriteModRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToFavourites([FromBody] string modId)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            await _favouriteModRepository.AddToFavourites(uid, modId);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavourites()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            return Ok(await _favouriteModRepository.GetFavourites(uid));
        }
    }
}
