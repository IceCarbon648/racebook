using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/favourite-mod")]
    public class FavouriteModController : Controller
    {
        private readonly IFavouriteModService _favouriteModService;

        public FavouriteModController(IFavouriteModService favouriteModService)
        {
            _favouriteModService = favouriteModService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToFavourites([FromBody] string modId)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            await _favouriteModService.AddToFavourites(uid, modId);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavourites()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            return Ok(await _favouriteModService.GetFavourites(uid));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteFromFavourites([FromBody] string modId)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            await _favouriteModService.DeleteFromFavourites(uid, modId);

            return Ok();
        }
    }
}
