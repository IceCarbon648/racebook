namespace Models.DTOs.Response
{
    public class ModDetailsDto
    {
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required string Creator { get; set; }
        public required string ModFileUrl { get; set; }
        public required string PreviewImageUrl { get; set; }
    }
}