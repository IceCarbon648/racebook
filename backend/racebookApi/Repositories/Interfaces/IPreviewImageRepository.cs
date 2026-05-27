namespace racebookApi.Repositories.Interfaces
{
    public interface IPreviewImageRepository
    {
        Task CreatePreviewImage(Guid modId, string imageUrl);
        Task DeletePreviewImage(string modId);
        Task<List<string>> GetPreviewImageUrl(string modId);
    }
}