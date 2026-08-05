using Ardalis.GuardClauses;
using Business.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Request;
using Models.DTOs.Response;

namespace Business
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;
        private readonly IFavouriteModRepository _favouriteModRepository;
        private readonly ILogger<ModService> _logger;

        const string PreviewImagesPublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, IFavouriteModRepository favouriteModRepository, ILogger<ModService> logger)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
            _favouriteModRepository = favouriteModRepository;
            _logger = logger;
        }

        public async Task UploadMod(string uid, ModDto dto)
        {
            _logger.LogInformation("User {Uid} uploading mod {Title}", uid, dto.Title);

            string modFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
            _logger.LogDebug("Mod file uploaded to Cloudinary for user {Uid}: {Url}", uid, modFileUrl);

            string previewImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);
            _logger.LogDebug("Preview image uploaded to Cloudinary for user {Uid}: {Url}", uid, previewImageUrl);

            Guid modId = await _modRepository.CreateMod(
                uid,
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                modFileUrl,
                previewImageUrl);

            _logger.LogInformation("Mod {ModId} successfully created by user {Uid}", modId, uid);
        }

        public async Task DeleteMod(string modId)
        {
            _logger.LogInformation("Deleting mod {ModId}", modId);

            await _favouriteModRepository.DeleteFavouriteModReference(modId);
            _logger.LogDebug("Favourite references removed for mod {ModId}", modId);

            Mod? mod = await _modRepository.DeleteMod(modId);
            Guard.Against.Null(mod, nameof(mod), $"Mod {modId} not found");
            _logger.LogDebug("Mod {ModId} deleted from database", modId);

            await _cloudinaryRepository.DeleteAsync(mod.ImageUrl, PreviewImagesPublicIdStart);
            _logger.LogDebug("Preview image deleted from Cloudinary for mod {ModId}", modId);

            await _cloudinaryRepository.DeleteAsync(mod.ModFileUrl, ModPublicIdStart);
            _logger.LogDebug("Mod file deleted from Cloudinary for mod {ModId}", modId);

            _logger.LogInformation("Mod {ModId} successfully deleted", modId);
        }

        public async Task EditMod(string modId, ModEditDto dto)
        {
            _logger.LogInformation("Editing mod {ModId}", modId);

            Mod? modDetails = await _modRepository.GetModById(modId);
            Guard.Against.Null(modDetails, nameof(modDetails), $"Mod {modId} not found");
            modDetails.EditDate = DateTime.Now;

            if (dto.PreviewImage != null)
            {
                string oldImageUrl = modDetails.ImageUrl;
                modDetails.ImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);
                _logger.LogDebug("New preview image uploaded for mod {ModId}: {Url}", modId, modDetails.ImageUrl);

                await _cloudinaryRepository.DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
                _logger.LogDebug("Old preview image deleted for mod {ModId}", modId);
            }

            if (dto.ModFile != null)
            {
                string oldModFileUrl = modDetails.ModFileUrl;
                modDetails.ModFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
                _logger.LogDebug("New mod file uploaded for mod {ModId}: {Url}", modId, modDetails.ModFileUrl);

                await _cloudinaryRepository.DeleteAsync(oldModFileUrl, ModPublicIdStart);
                _logger.LogDebug("Old mod file deleted for mod {ModId}", modId);
            }

            await _modRepository.EditMod(modDetails, dto.Title, dto.Type, dto.Description);
            _logger.LogInformation("Mod {ModId} successfully edited", modId);
        }

        public async Task<List<GetModDto>> GetAllMods()
        {
            _logger.LogInformation("Retrieving all mods");
            List<GetModDto> mods = await _modRepository.GetAllMods();

            if (mods.Count == 0)
            {
                _logger.LogInformation("No mods found");
            }
            else
            {
                _logger.LogInformation("Retrieved {Count} mods", mods.Count);
            }

            return mods;
        }

        public async Task<List<Mod>> GetMyMods(string uid)
        {
            _logger.LogInformation("Retrieving mods for user {Uid}", uid);
            List<Mod> mods = await _modRepository.GetMyMods(uid);

            if (mods.Count == 0)
            {
                _logger.LogInformation("No mods found for user {Uid}", uid);
            }
            else
            {
                _logger.LogInformation("Retrieved {Count} mods for user {Uid}", mods.Count, uid);
            }

            return mods;
        }
    }
}