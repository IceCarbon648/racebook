namespace racebookApi.Repositories.Interfaces
{
    public interface IPreviewImageRepository
    {
        Task CreatePreviewImage(Guid modId, string imageUrl);
    }
}