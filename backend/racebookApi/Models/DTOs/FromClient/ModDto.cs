namespace racebookApi.Models.DTOs.FromClient
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