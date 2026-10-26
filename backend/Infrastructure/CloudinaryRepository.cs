using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class CloudinaryRepository : ICloudinaryRepository
    {
        private readonly Cloudinary _cloudinary;

        private readonly Dictionary<FileType, string> _folders = new()
        {
            { FileType.Image, "PreviewImages" },
            { FileType.Raw,   "Mods"  }
        };

        public CloudinaryRepository(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<RawUploadResult> UploadAsync(IFormFile file, FileType fileType)
        {
            using Stream stream = file.OpenReadStream();

            RawUploadParams uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = _folders[fileType]
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        private string GetPublicIdFromUrl(string cloudniaryUrl, string publicIdStart)
        {
            int publicIdStartIndex = cloudniaryUrl.IndexOf(publicIdStart);

            return cloudniaryUrl.Substring(publicIdStartIndex);
        }

        public async Task DeleteAsync(string fileUrl, string publicIdStart)
        {
            string filePublicId = GetPublicIdFromUrl(fileUrl, publicIdStart);

            await _cloudinary.DestroyAsync(new DeletionParams(filePublicId)
            {
                ResourceType = ResourceType.Raw
            });
        }
    }
}