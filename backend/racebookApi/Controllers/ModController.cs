using Business.Interfaces;
using Business.Models.DTOs.Request;
using Business.Models.Validators.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/mods")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;

        public ModController(IModService modService)
        {
            _modService = modService;
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<ModDto>))]
        public async Task<IActionResult> UploadMod([FromForm] ModDto dto)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            await _modService.UploadMod(uid, dto);

            return Ok();
        }

        [HttpDelete("{modId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMod([FromRoute] string modId)
        {
            await _modService.DeleteMod(modId);

            return Ok();
        }

        [HttpPatch("{modId}")]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<ModEditDto>))]
        public async Task<IActionResult> EditMod([FromRoute] string modId, [FromForm] ModEditDto dto)
        {
            await _modService.EditMod(modId, dto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMods()
        {
            return Ok(await _modService.GetAllMods());
        }

        [HttpGet("my-mods")]
        [Authorize]
        public async Task<IActionResult> GetMyMods()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            return Ok(await _modService.GetMyMods(uid!));
        }
    }
}