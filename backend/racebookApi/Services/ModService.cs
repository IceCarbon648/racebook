using racebookApi.Constants;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using CloudinaryDotNet.Actions;

namespace racebookApi.Services
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;
        private readonly IPreviewImageRepository _previewImageRepository;

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, IPreviewImageRepository previewImageRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
            _previewImageRepository = previewImageRepository;
        }

        public async Task<string> UploadModFile(IFormFile modFile)
        {
            return await _cloudinaryRepository.UploadAsync(modFile, FileType.Raw);
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

        public async Task<Guid> SaveModFile(string uid, string title, string type, string description, string modFileUrl)
        {
            return await _modRepository.CreateMod(uid, title, type, description, DateOnly.FromDateTime(DateTime.Now).ToString(), DateOnly.FromDateTime(DateTime.Now).ToString(), modFileUrl);
        }

        public async Task SavePreviewImages(Guid modId, List<string> previewImageUrls)
        {
            foreach (string previewImageUrl in previewImageUrls)
            {
                await _previewImageRepository.CreatePreviewImage(modId, previewImageUrl);
            }
        }

        public async Task DeleteMod(string modId)
        {
            const string PreviewImagesPublicIdStart = "PreviewImages";
            const string ModPublicIdStart = "Mods";

            string modMediaPublicId = "";

            List<string> previewImageUrls = await _previewImageRepository.GetPreviewImageUrl(modId);
            string modFileUrl = await _modRepository.GetModFileUrl(modId);

            foreach (string previewImageUrl in previewImageUrls)
            {
                modMediaPublicId = GetPublicIdFromUrl(previewImageUrl, PreviewImagesPublicIdStart);
                await DeleteFromCloudinaryByPublicId(modMediaPublicId);
            }

            await _previewImageRepository.DeletePreviewImage(modId);

            modMediaPublicId = GetPublicIdFromUrl(modFileUrl, ModPublicIdStart);
            await DeleteFromCloudinaryByPublicId(modMediaPublicId);

            await _modRepository.DeleteMod(modId);
        }

        private string GetPublicIdFromUrl(string cloudniaryUrl, string publicIdStart)
        {
            int publicIdStartIndex = cloudniaryUrl.IndexOf(publicIdStart);

            return cloudniaryUrl.Substring(publicIdStartIndex);
        }

        private async Task DeleteFromCloudinaryByPublicId(string publicId)
        {
            await _cloudinaryRepository.DeleteAsync(new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            });
        }
    }
}