namespace racebookApi.Models
{
    public class PreviewImages
    {
        public Guid PreviewImageId { get; set; }
        public required List<Mod> Mods { get; set; }
        public required string FilePath { get; set;  }
    }
}