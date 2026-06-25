using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;
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
        public async Task<IActionResult> EditMod([FromRoute] string modId, [FromForm] ModEditDto dto)
        {
            await _modService.EditMod(modId, dto);

            return Ok();
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromBody] string modFileUrl)
        {
            return File(await _modService.DownloadModFile(modFileUrl), "application/octet-stream");
        }

        [HttpGet("{modId}")]
        public async Task<IActionResult> GetMod([FromRoute] string modId)
        {
            return Ok(await _modService.GetMod(modId));
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