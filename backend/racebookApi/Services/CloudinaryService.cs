using racebookApi.Constants;
using racebookApi.Repositories.Interfaces;

namespace racebookApi.Services
{
    public class CloudinaryService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public CloudinaryService(ICloudinaryRepository cloudinaryRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
        }

        public async Task<string> UploadMod(IFormFile mod)
        {
            return await _cloudinaryRepository.UploadAsync(mod, FileType.Raw);
        }

        public async Task<List<string>> UploadPreviewImages(List<IFormFile> previewImages)
        {
            List<string> previewImageUrls = new List<string>();

            foreach (IFormFile previewImage in previewImages)
            {
                previewImageUrls.Add(await _cloudinaryRepository.UploadAsync(previewImage, FileType.Image));
            }

            return previewImageUrls;
        }
    }
}