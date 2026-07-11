using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public interface ICloudinaryRepository
    {
        Task<string> UploadAsync(IFormFile file, FileType fileType);
        Task DeleteAsync(DeletionParams deletionParams);
    }
}