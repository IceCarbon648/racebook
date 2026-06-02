using CloudinaryDotNet.Actions;
using racebookApi.Constants;
using racebookApi.Models;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Models.DTOs.ToClient;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using System.Reflection;
using System.Security.Cryptography;

namespace racebookApi.Services
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;
        private readonly IPreviewImageRepository _previewImageRepository;
        private readonly IUserRepository _userRepository;

        const string PreviewImagesPublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, IPreviewImageRepository previewImageRepository, IUserRepository userRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
            _previewImageRepository = previewImageRepository;
            _userRepository = userRepository;
        }

        public async Task UploadMod(ModDto dto)
        {
            string modFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
            List<string> previewImageUrls = await UploadPreviewImages(dto.PreviewImages);

            Guid modId = await _modRepository.CreateMod(
                "9D51DE57-A958-4B74-B975-52A5F81C7F93",
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                modFileUrl);

            await SavePreviewImages(modId, previewImageUrls);
        }

        private async Task<List<string>> UploadPreviewImages(List<IFormFile> previewImages)
        {
            List<string> previewImageUrls = new List<string>();

            foreach (IFormFile previewImage in previewImages)
            {
                previewImageUrls.Add(await _cloudinaryRepository.UploadAsync(previewImage, FileType.Image));
            }

            return previewImageUrls;
        }

        private async Task SavePreviewImages(Guid modId, List<string> previewImageUrls)
        {
            foreach (string previewImageUrl in previewImageUrls)
            {
                await _previewImageRepository.CreatePreviewImage(modId, previewImageUrl);
            }
        }

        public async Task DeleteMod(string modId)
        {
            List<string> previewImageUrls = await _previewImageRepository.GetPreviewImageUrl(modId);
            string modFileUrl = await _modRepository.GetModFileUrl(modId);

            await DeletePreviewImages(previewImageUrls, PreviewImagesPublicIdStart);
            await _previewImageRepository.DeletePreviewImageByModId(modId);

            await DeleteModFile(modFileUrl, ModPublicIdStart);
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

        private async Task DeletePreviewImages(List<string> previewImageUrls, string publicIdStart)
        {
            string imagePublicId = "";

            foreach (string previewImageUrl in previewImageUrls)
            {
                imagePublicId = GetPublicIdFromUrl(previewImageUrl, publicIdStart);
                await DeleteFromCloudinaryByPublicId(imagePublicId);
            }
        }

        private async Task DeleteModFile(string modFileUrl, string publicIdStart)
        {
            string modFilePublicId = GetPublicIdFromUrl(modFileUrl, ModPublicIdStart);
            await DeleteFromCloudinaryByPublicId(modFilePublicId);
        }

        public async Task EditMod(ModEditDto dto)
        {
            Mod modDetails = await _modRepository.GetModById(dto.ModId.ToString());

            if (dto.PreviewImagesToBeDeleted != null)
            {
                await DeletePreviewImages(dto.PreviewImagesToBeDeleted, PreviewImagesPublicIdStart);

                foreach (string previewImageUrl in dto.PreviewImagesToBeDeleted)
                {
                    await _previewImageRepository.DeletePreviewImageByUrl(previewImageUrl);
                }
            }

            if (dto.NewPreviewImages != null)
            {
                List<string> previewImageUrls = await UploadPreviewImages(dto.NewPreviewImages);
                await SavePreviewImages(dto.ModId, previewImageUrls);
            }

            if (dto.ModFile != null)
            {
                string oldModFileUrl = await _modRepository.GetModFileUrl(dto.ModId.ToString());
                await DeleteModFile(oldModFileUrl, ModPublicIdStart);

                string newModFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
                modDetails.FilePath = newModFileUrl;
            }

            if (dto.Description != null)
            {
                modDetails.Description = dto.Description;
            }

            if (dto.Type != null)
            {
                modDetails.Type = dto.Type;
            }

            if (dto.Title != null)
            {
                modDetails.Title = dto.Title;
            }

            modDetails.EditDate = DateTime.Now;

            await _modRepository.EditMod(modDetails);
        }

        public async Task<byte[]?> DownloadModFile(string modFileUrl)
        {
            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(modFileUrl);

            if (!response.IsSuccessStatusCode) return null;//ASK DAMIAN. . .prolly needs adapter

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<GetModDto> GetMod(string modId)
        {
            Mod modInfo = await _modRepository.GetModById(modId);
            string username = await _userRepository.GetUsernameByUserId(modInfo.Uid.ToString());
            List<string> previewImageUrls = await _previewImageRepository.GetPreviewImageUrl(modId);

            return new GetModDto
            {
                Id = modId,
                Creator = username,
                Title = modInfo.Title,
                Type = modInfo.Type,
                Description = modInfo.Description,
                UploadDate = modInfo.UploadDate,
                EditDate = modInfo.EditDate,
                ModFileUrl = modInfo.FilePath,
                PreviewImageUrls = previewImageUrls,
            };
        }

        public async Task<List<GetModDto>> GetAllMods()
        {
            List<GetModDto> allMods = new List<GetModDto>();
            List<Guid> modIds = await _modRepository.GetAllModIds();

            foreach (Guid modId in modIds)
            {
                allMods.Add(await GetMod(modId.ToString()));
            }

            return allMods;
        }

        public async Task<List<GetModDto>> GetMyMods(string uid)
        {
            List<GetModDto> myMods = new List<GetModDto>();
            List<Guid> myModIds = await _modRepository.GetMyModIds(uid);

            foreach (Guid modId in myModIds)
            {
                myMods.Add(await GetMod(modId.ToString()));
            }

            return myMods;
        }
    }
}