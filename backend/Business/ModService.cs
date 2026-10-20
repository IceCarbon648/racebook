using Ardalis.GuardClauses;
using Business.Interfaces;
using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using MagicBytesValidator.Formats;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Request;
using Models.DTOs.Response;
using System.Diagnostics.CodeAnalysis;
using System.Net;

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

        public async Task<Guid?> UploadMod(string uid, ModDto dto)
        {
            _logger.LogInformation("User {Uid} uploading mod {Title}", uid, dto.Title);

            RawUploadResult modFileUploadResult = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);

            if (modFileUploadResult.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("Failed to upload mod file. {StatusCode}: {ErrorMessage}", modFileUploadResult.StatusCode, modFileUploadResult.Error.Message);

                return null;
            }

            _logger.LogDebug("Mod file uploaded to Cloudinary for user {Uid}: {Url}", uid, modFileUploadResult.SecureUrl);

            RawUploadResult previewImageUploadResult = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);

            if (previewImageUploadResult.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("Failed to upload preview image. {StatusCode}: {ErrorMessage}", previewImageUploadResult.StatusCode, previewImageUploadResult.Error.Message);

                await _cloudinaryRepository.DeleteAsync(modFileUploadResult.SecureUrl.ToString(), ModPublicIdStart);
                _logger.LogWarning("Removed orphaned mod file for user {Uid}", uid);

                return null;
            }
            _logger.LogDebug("Preview image uploaded to Cloudinary for user {Uid}: {Url}", uid, previewImageUploadResult.SecureUrl);

            Guid modId = await _modRepository.CreateMod(
                uid,
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                modFileUploadResult.SecureUrl.ToString(),
                previewImageUploadResult.SecureUrl.ToString());

            _logger.LogInformation("Mod {ModId} successfully created by user {Uid}", modId, uid);

            return modId;
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

        public async Task<bool> EditMod(string modId, ModEditDto dto)
        {
            _logger.LogInformation("Editing mod {ModId}", modId);

            Mod? modDetails = await _modRepository.GetModById(modId);
            Guard.Against.Null(modDetails, nameof(modDetails), $"Mod {modId} not found");

            List<(string Url, string PublicIdStart)> oldFiles = [];
            List<(string Url, string PublicIdStart)> newFiles = [];

            if (dto.PreviewImage is not null)
            {
                RawUploadResult result = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError("Failed to upload preview image for mod {ModId}. {StatusCode}: {ErrorMessage}", modId, result.StatusCode, result.Error?.Message);

                    return false;
                }

                oldFiles.Add((modDetails.ImageUrl, PreviewImagesPublicIdStart));
                newFiles.Add((result.SecureUrl.ToString(), PreviewImagesPublicIdStart));
                modDetails.ImageUrl = result.SecureUrl.ToString();

                _logger.LogDebug("New preview image uploaded for mod {ModId}: {Url}", modId, modDetails.ImageUrl);
            }

            if (dto.ModFile is not null)
            {
                RawUploadResult result = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError("Failed to upload mod file for mod {ModId}. {StatusCode}: {ErrorMessage}", modId, result.StatusCode, result.Error?.Message);
                    await DeleteFiles(newFiles);

                    return false;
                }

                oldFiles.Add((modDetails.ModFileUrl, ModPublicIdStart));
                newFiles.Add((result.SecureUrl.ToString(), ModPublicIdStart));

                modDetails.ModFileUrl = result.SecureUrl.ToString();

                _logger.LogDebug("New mod file uploaded for mod {ModId}: {Url}", modId, modDetails.ModFileUrl);
            }

            modDetails.EditDate = DateTime.Now;

            int rowsAffected;

            try
            {
                rowsAffected = await _modRepository.EditMod(modDetails, dto.Title, dto.Type, dto.Description);
            }
            catch
            {
                await DeleteFiles(newFiles);
                throw;
            }

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Edit of mod {ModId} affected no rows, rolling back uploads", modId);
                await DeleteFiles(newFiles);
                return false;
            }

            await DeleteFiles(oldFiles);

            _logger.LogInformation("Mod {ModId} successfully edited", modId);
            return true;
        }

        public async Task<List<GetModDto>> GetAllMods(string? uid)
        {
            _logger.LogInformation("Retrieving all mods");
            List<GetModDto> mods = await _modRepository.GetAllMods(uid);

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

        public async Task<bool> IsOwner(string modId, string uid)
        {
            _logger.LogInformation("Checking if user {Uid} is the owner of mod {ModId}", uid, modId);

            bool isOwner = await _modRepository.IsOwner(modId, uid);

            _logger.LogInformation("User {Uid} owns mod {ModId}: {IsOwner}", uid, modId, isOwner);

            return isOwner;
        }

        private async Task DeleteFiles(IEnumerable<(string Url, string PublicIdStart)> files)
        {
            foreach ((string url, string publicIdStart) in files)
            {
                await _cloudinaryRepository.DeleteAsync(url, publicIdStart);
            }
        }
    }
}