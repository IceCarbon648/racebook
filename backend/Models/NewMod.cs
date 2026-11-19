namespace Models
{
    public class NewMod
    {
        public required string Uid { get; set; }
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Description { get; set; }
        public required string UploadDate { get; set; }
        public required string EditDate { get; set; }
        public required string ModFileUrl {  get; set; }
        public required string ImageUrl { get; set; }
    }
}