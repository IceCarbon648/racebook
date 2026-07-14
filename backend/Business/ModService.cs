using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Infrastructure.Models;
using Business.Models.DTOs.Request;
using Infrastructure.Models.DTOs.Response;
using Infrastructure.Interfaces;
using Business.Interfaces;

namespace Business
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;

        const string PreviewImagesPublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
        }

        public async Task UploadMod(string uid, ModDto dto)
        {
            string modFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
            string previewImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);

            Guid modId = await _modRepository.CreateMod(
                uid,
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                modFileUrl,
                previewImageUrl);
        }

        public async Task DeleteMod(string modId)
        {
            Mod mod = await _modRepository.DeleteMod(modId);

            await DeleteFromCloudinary(mod.ImageUrl, PreviewImagesPublicIdStart);
            await DeleteFromCloudinary(mod.ModFileUrl, ModPublicIdStart);
        }

        private string GetPublicIdFromUrl(string cloudniaryUrl, string publicIdStart)
        {
            int publicIdStartIndex = cloudniaryUrl.IndexOf(publicIdStart);

            return cloudniaryUrl.Substring(publicIdStartIndex);
        }

        private async Task DeleteFromCloudinary(string fileUrl, string publicIdStart)
        {//ASK DAMIAN: MOVE THIS GUY TO ITS OWN SERVICE OR NAH?
            string filePublicId = GetPublicIdFromUrl(fileUrl, publicIdStart);

            await _cloudinaryRepository.DeleteAsync(new DeletionParams(filePublicId)
            {
                ResourceType = ResourceType.Raw
            });
        }

        public async Task EditMod(string modId, ModEditDto dto)
        {
            Mod modDetails = await _modRepository.GetModById(modId);
            modDetails.EditDate = DateTime.Now;

            if (dto.PreviewImage != null)
            {
                await DeleteFromCloudinary(modDetails.ImageUrl, PreviewImagesPublicIdStart);
                modDetails.ImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);
            }

            if (dto.ModFile != null)
            {
                await DeleteFromCloudinary(modDetails.ModFileUrl, ModPublicIdStart);
                modDetails.ModFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
            }

            await _modRepository.EditMod(modDetails, dto.Title, dto.Type, dto.Description);
        }

        public async Task<List<GetModDto>> GetAllMods()
        {
            return await _modRepository.GetAllMods();
        }

        public async Task<List<Mod>> GetMyMods(string uid)
        {
            return await _modRepository.GetMyMods(uid);
        }
    }
}