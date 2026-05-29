using racebookApi.Constants;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using CloudinaryDotNet.Actions;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Models;
using racebookApi.Models.DTOs.ToClient;

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
            string modFileUrl = await UploadModFile(dto.ModFile);
            List<string> previewImageUrls = await UploadPreviewImages(dto.PreviewImages);

            Guid modId = await SaveModFile("9D51DE57-A958-4B74-B975-52A5F81C7F93", dto.Title, dto.Type, dto.Description, modFileUrl);
            await SavePreviewImages(modId, previewImageUrls);
        }

        private async Task<string> UploadModFile(IFormFile modFile)
        {
            return await _cloudinaryRepository.UploadAsync(modFile, FileType.Raw);
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

        private async Task<Guid> SaveModFile(string uid, string title, string type, string description, string modFileUrl)
        {
            return await _modRepository.CreateMod(uid, title, type, description, DateOnly.FromDateTime(DateTime.Now).ToString(), DateOnly.FromDateTime(DateTime.Now).ToString(), modFileUrl);
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

                string newModFileUrl = await UploadModFile(dto.ModFile);
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

        public async Task<GetModDto> GetMod(string modID)
        {
            Mod modInfo = await _modRepository.GetModById(modID);
            string username = await _userRepository.GetUsernameByUserId(modInfo.Uid.ToString());
            List<string> previewImageUrls = await _previewImageRepository.GetPreviewImageUrl(modID);

            return new GetModDto
            {
                Id = modID,
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
            List<string> modIds = await _modRepository.GetAllModIds();

            foreach (string modId in modIds)
            {
                allMods.Add(await GetMod(modId));
            }

            return allMods;
        }
    }
}