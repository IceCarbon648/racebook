using Microsoft.AspNetCore.Http;

namespace Business.Models.DTOs.Request
{
    public class ModDto
    {
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required IFormFile ModFile { get; set; }
        public required List<IFormFile> PreviewImages { get; set; }
    }
}