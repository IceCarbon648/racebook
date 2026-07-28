using Microsoft.AspNetCore.Http;

namespace Models.DTOs.Request
{
    public class ModEditDto
    {
        public string? Title { get; set; } = null;
        public string? Type { get; set; } = null;
        public string? Description { get; set; } = null;
        public IFormFile? ModFile { get; set; } = null;
        public IFormFile? PreviewImage { get; set; } = null;
    }
}