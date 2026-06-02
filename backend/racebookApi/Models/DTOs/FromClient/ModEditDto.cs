namespace racebookApi.Models.DTOs.FromClient
{
    public class ModEditDto
    {
        public required Guid ModId { get; set; }
        public string? Title { get; set; } = null;
        public string? Type { get; set; } = null;
        public string? Description { get; set; } = null;
        public IFormFile? ModFile { get; set; } = null;
        public List<IFormFile>? NewPreviewImages { get; set; } = null;
        public List<string>? PreviewImagesToBeDeleted { get; set; } = null;
    }
}