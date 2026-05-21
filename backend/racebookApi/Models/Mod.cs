namespace racebookApi.Models
{
    public class Mod
    {
        public Guid ModId { get; set; }
        public required List<User> Users { get; set; }
        public required string Title { get; set;  }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required DateTime UploadDate { get; set; }
        public required DateTime EditDate { get; set; }
        public required string FilePath { get; set; }
    }
}