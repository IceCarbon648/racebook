using Microsoft.AspNetCore.Mvc;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Services.Interfaces;

namespace racebookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;

        public ModController(IModService modService)
        {
            _modService = modService;
        }

        [HttpPost("Mod")]
        public async Task<IActionResult> UploadMod([FromForm] ModDto dto)
        {
            await _modService.UploadMod(dto);

            return Ok();
        }

        [HttpDelete("Mod/{modId}")]
        public async Task<IActionResult> DeleteMod([FromRoute] string modId)
        {
            await _modService.DeleteMod(modId);

            return Ok();
        }

        [HttpPatch("Mod/ammend")]
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
    }
}