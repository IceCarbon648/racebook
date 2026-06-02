using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;

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
        public async Task<IActionResult> UploadMod([FromForm] ModDto dto)
        {
            await _modService.UploadMod(dto);

            return Ok();
        }

        [HttpDelete("{modId}")]
        public async Task<IActionResult> DeleteMod([FromRoute] string modId)
        {
            await _modService.DeleteMod(modId);

            return Ok();
        }

        [HttpPatch("ammend")]
        public async Task<IActionResult> EditMod([FromForm] ModEditDto dto)
        {
            await _modService.EditMod(dto);

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
        public async Task<IActionResult> GetMyMods()
        {
            return Ok(await _modService.GetMyMods("9D51DE57-A958-4B74-B975-52A5F81C7F93"));
        }
    }
}