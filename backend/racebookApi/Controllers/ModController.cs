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
            string modFileUrl = await _modService.UploadModFile(dto.ModFile);
            List<string> previewImageUrls = await _modService.UploadPreviewImages(dto.PreviewImages);

            Guid modId = await _modService.SaveModFile("9D51DE57-A958-4B74-B975-52A5F81C7F93", dto.Title, dto.Type, dto.Description, modFileUrl);
            await _modService.SavePreviewImages(modId, previewImageUrls);

            return Ok();
        }

        [HttpDelete("Mod/{modId}")]
        public async Task<IActionResult> DeleteMod([FromRoute] string modId)
        {
            await _modService.DeleteMod(modId);

            return Ok();
        }
    }
}