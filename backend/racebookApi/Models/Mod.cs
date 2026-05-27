namespace racebookApi.Models
{
    public class Mod
    {
        public Guid ModId { get; set; }
        public Guid Uid { get; set; }
        public required string Title { get; set;  }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required DateOnly UploadDate { get; set; }
        public required DateOnly EditDate { get; set; }
        public required string FilePath { get; set; }
    }
}