using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public interface ICloudinaryRepository
    {
        Task<RawUploadResult> UploadAsync(IFormFile file, FileType fileType);
        Task DeleteAsync(string fileUrl, string publicIdStart);
    }
}