using racebookApi.Constants;

namespace racebookApi.Services.Interfaces
{
    public interface IModService
    {
        Task<string> UploadModFile(IFormFile modFile);
        Task<List<string>> UploadPreviewImages(List<IFormFile> previewImages);
        Task<Guid> SaveModFile(string uid, string title, string type, string description, string modFileUrl);
        Task SavePreviewImages(Guid modId, List<string> previewImageUrls);
    }
}