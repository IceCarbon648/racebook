using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Request;
using Models.Validators.Filter;
using System.Security.Claims;
using Models.Constants;

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

            Guid? modId = await _modService.UploadMod(uid, dto);

            if (modId is null)
            {
                return StatusCode(StatusCodes.Status502BadGateway,
                    new { message = "Upload service is unavailable, please try again" });
            }

            return Created($"/api/mod/{modId}", new { modId });
        }

        [HttpDelete("{modId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMod([FromRoute] string modId)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            if (!await _modService.IsOwner(modId, uid))
            {
                return Forbid(Constants.NotOwnerMessage);
            }

            await _modService.DeleteMod(modId);

            return Ok();
        }

        [HttpPatch("{modId}")]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<ModEditDto>))]
        public async Task<IActionResult> EditMod([FromRoute] string modId, [FromForm] ModEditDto dto)
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            if (!await _modService.IsOwner(modId, uid))
            {
                return Forbid(Constants.NotOwnerMessage);
            }

            await _modService.EditMod(modId, dto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMods()
        {
            string? uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            return Ok(await _modService.GetAllMods(uid));
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