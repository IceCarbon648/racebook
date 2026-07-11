namespace Infrastructure.Interfaces
{
    public interface IPreviewImageRepository
    {
        Task CreatePreviewImage(Guid modId, string imageUrl);
        Task DeletePreviewImageByModId(string modId);
        Task<List<string>> GetPreviewImageUrl(string modId);
        Task DeletePreviewImageByUrl(string Url);
    }
}