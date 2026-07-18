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
        private readonly IFavouriteModRepository _favouriteModRepository;

        const string PreviewImagesPublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, IFavouriteModRepository favouriteModRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
            _favouriteModRepository = favouriteModRepository;
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
            await _favouriteModRepository.DeleteFavouriteModReference(modId);
            Mod mod = await _modRepository.DeleteMod(modId);

            await _cloudinaryRepository.DeleteAsync(mod.ImageUrl, PreviewImagesPublicIdStart);
            await _cloudinaryRepository.DeleteAsync(mod.ModFileUrl, ModPublicIdStart);
        }

        public async Task EditMod(string modId, ModEditDto dto)
        {
            Mod modDetails = await _modRepository.GetModById(modId);
            modDetails.EditDate = DateTime.Now;

            if (dto.PreviewImage != null)
            {
                modDetails.ImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);
                await _cloudinaryRepository.DeleteAsync(modDetails.ImageUrl, PreviewImagesPublicIdStart);
            }

            if (dto.ModFile != null)
            {
                modDetails.ModFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
                await _cloudinaryRepository.DeleteAsync(modDetails.ModFileUrl, ModPublicIdStart);
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