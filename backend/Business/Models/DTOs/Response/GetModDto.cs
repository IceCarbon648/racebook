namespace Business.Models.DTOs.Response
{
    public class GetModDto
    {
        public required string Id { get; set; }
        public required string Creator { get; set; }
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required DateTime UploadDate { get; set; }
        public required DateTime EditDate { get; set; }
        public required string ModFileUrl { get; set; }
        public required string PreviewImageUrl { get; set; }
    }
}