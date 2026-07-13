namespace Infrastructure.Models
{
    public class Mod
    {
        public Guid ModId { get; set; }
        public Guid Uid { get; set; }
        public required string Title { get; set;  }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required DateTime UploadDate { get; set; }
        public required DateTime EditDate { get; set; }
        public required string ModFileUrl { get; set; }
        public required string ImageUrl { get; set; }
    }
}