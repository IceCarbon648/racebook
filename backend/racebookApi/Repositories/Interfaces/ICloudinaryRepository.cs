using CloudinaryDotNet.Actions;
using racebookApi.Constants;

namespace racebookApi.Repositories.Interfaces
{
    public interface ICloudinaryRepository
    {
        Task<string> UploadAsync(IFormFile file, FileType fileType);
        Task DeleteAsync(DeletionParams deletionParams);
    }
}