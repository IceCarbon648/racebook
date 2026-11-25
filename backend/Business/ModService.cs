using Ardalis.GuardClauses;
using Business.Interfaces;
using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Request;
using Models.DTOs.Response;
using System.Net;

namespace Business
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;
        private readonly ILogger<ModService> _logger;
        private readonly record struct CloudinaryFile(string Url, string PublicIdStart);

        const string PreviewImagePublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, ILogger<ModService> logger)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
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

            NewMod newMod = new NewMod {
                                           Uid = uid,
                                           Title = dto.Title,
                                           Type = dto.Type,
                                           Description = dto.Description,
                                           UploadDate = DateOnly.FromDateTime(DateTime.Now).ToString(),
                                           EditDate = DateOnly.FromDateTime(DateTime.Now).ToString(),
                                           ModFileUrl = modFileUploadResult.SecureUrl.ToString(),
                                           ImageUrl = previewImageUploadResult.SecureUrl.ToString()
                                       };

            Guid modId = await _modRepository.CreateMod(newMod);

            _logger.LogInformation("Mod {ModId} successfully created by user {Uid}", modId, uid);

            return modId;
        }

        public async Task DeleteMod(string modId)
        {
            _logger.LogInformation("Deleting mod {ModId}", modId);

            Mod? mod = await _modRepository.DeleteMod(modId);
            Guard.Against.Null(mod, nameof(mod), $"Mod {modId} not found");

            _logger.LogDebug("Mod {ModId} deleted from database", modId);

            await _cloudinaryRepository.DeleteAsync(mod.ImageUrl, PreviewImagePublicIdStart);
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

            string? latestModFileUrl = modDetails.ModFileUrl;
            string? latestImageUrl = modDetails.ImageUrl;

            if (dto.ModFile is not null)
            {
                _logger.LogInformation("Attempting to upload new mod file");

                latestModFileUrl = await TryUploadToCloudinary(dto.ModFile, FileType.Raw);
            }

            if (latestModFileUrl is null)
            {
                _logger.LogWarning("Failed to upload new mod file");

                return false;
            }

            if (dto.PreviewImage is not null)
            {
                _logger.LogInformation("Attempting to upload new image file");

                latestImageUrl = await TryUploadToCloudinary(dto.PreviewImage, FileType.Image);
            }

            if (latestImageUrl is null && dto.ModFile is not null)
            {
                _logger.LogWarning("Failed to upload new image file");
                _logger.LogWarning("Attempting to delete new mod file");

                CloudinaryFile newModFile = new CloudinaryFile
                {
                    Url = latestModFileUrl,
                    PublicIdStart = ModPublicIdStart
                };

                await TryDeleteFromCloudinary(newModFile);

                return false;
            }
            else if (latestImageUrl is null)
            {
                _logger.LogWarning("Failed to upload new image file");

                return false;
            }

            if (dto.ModFile is not null)
            {
                CloudinaryFile oldModFile = new CloudinaryFile
                {
                    Url = modDetails.ModFileUrl,
                    PublicIdStart = ModPublicIdStart
                };

                _logger.LogInformation("Deleting old mod file");
                await TryDeleteFromCloudinary(oldModFile);
            }

            if (dto.PreviewImage is not null)
            {
                CloudinaryFile oldImageFile = new CloudinaryFile
                {
                    Url = modDetails.ImageUrl,
                    PublicIdStart = PreviewImagePublicIdStart
                };

                _logger.LogInformation("Deleting old image file");
                await TryDeleteFromCloudinary(oldImageFile);
            }

            modDetails.Title = dto.Title;
            modDetails.Type = dto.Type;
            modDetails.Description = dto.Description;
            modDetails.EditDate = DateTime.Now;
            modDetails.ModFileUrl = latestModFileUrl;
            modDetails.ImageUrl = latestImageUrl;

            await _modRepository.EditMod(modDetails);

            _logger.LogInformation("Mod {modId} edited successfully", modId);

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

        public async Task<ModDetailsDto> GetModDetails(string modId)
        {
            _logger.LogInformation("Retrieving mod {modId}", modId);

            ModDetailsDto? modDetails = await _modRepository.GetModDetails(modId);

            Guard.Against.Null(modDetails, nameof(modDetails), $"Mod {modId} not found");

            return modDetails!;
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

        private async Task<string?> TryUploadToCloudinary(IFormFile file, FileType type)
        {
            RawUploadResult uploadResult = await _cloudinaryRepository.UploadAsync(file, type);

            return uploadResult.StatusCode == HttpStatusCode.OK? uploadResult.SecureUrl.ToString() : null;
        }

        private async Task<bool> TryDeleteFromCloudinary(CloudinaryFile cloudinaryFile)
        {
            DeletionResult deletionResult = await _cloudinaryRepository.DeleteAsync(cloudinaryFile.Url, cloudinaryFile.PublicIdStart);

            return deletionResult.StatusCode == HttpStatusCode.OK;
        }
    }
}