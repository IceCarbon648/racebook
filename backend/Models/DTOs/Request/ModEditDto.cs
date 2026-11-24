using Microsoft.AspNetCore.Http;

namespace Models.DTOs.Request
{
    public record ModEditDto
    {
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public IFormFile? ModFile { get; set; } = null;
        public IFormFile? PreviewImage { get; set; } = null;
    }
}