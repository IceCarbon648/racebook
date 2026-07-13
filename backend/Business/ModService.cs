using CloudinaryDotNet.Actions;
using Infrastructure.Constants;
using Infrastructure.Models;
using Business.Models.DTOs.Request;
using Business.Models.DTOs.Response;
using Infrastructure.Interfaces;
using Business.Interfaces;

namespace Business
{
    public class ModService : IModService
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;
        private readonly IModRepository _modRepository;
        private readonly IUserRepository _userRepository;

        const string PreviewImagesPublicIdStart = "PreviewImages";
        const string ModPublicIdStart = "Mods";

        public ModService(ICloudinaryRepository cloudinaryRepository, IModRepository modRepository, IUserRepository userRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
            _modRepository = modRepository;
            _userRepository = userRepository;
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

            if (dto.PreviewImage != null)
            {
                await DeleteFromCloudinary(modDetails.ImageUrl, PreviewImagesPublicIdStart);

                string newImageUrl = await _cloudinaryRepository.UploadAsync(dto.PreviewImage, FileType.Image);
                modDetails.ImageUrl = newImageUrl;
            }

            if (dto.ModFile != null)
            {
                await DeleteFromCloudinary(modDetails.ModFileUrl, ModPublicIdStart);

                string newModFileUrl = await _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw);
                modDetails.ModFileUrl = newModFileUrl;
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

        public async Task<GetModDto> GetMod(string modId)
        {
            Mod modInfo = await _modRepository.GetModById(modId);
            string username = await _userRepository.GetUsernameByUserId(modInfo.Uid.ToString());

            return new GetModDto
            {
                Id = modId,
                Creator = username,
                Title = modInfo.Title,
                Type = modInfo.Type,
                Description = modInfo.Description,
                UploadDate = modInfo.UploadDate,
                EditDate = modInfo.EditDate,
                ModFileUrl = modInfo.ModFileUrl,
                PreviewImageUrl = modInfo.ImageUrl
            };
        }

        private async Task<List<GetModDto>> GetModsById(List<Guid> modIds)
        {
            List<GetModDto> mods = new List<GetModDto>();

            foreach (Guid modId in modIds)
            {
                mods.Add(await GetMod(modId.ToString()));
            }

            return mods;
        }

        public async Task<List<GetModDto>> GetAllMods()
        {
            List<Guid> modIds = await _modRepository.GetAllModIds();

            return await GetModsById(modIds);
        }

        public async Task<List<GetModDto>> GetMyMods(string uid)
        {
            List<Guid> modIds = await _modRepository.GetMyModIds(uid);

            return await GetModsById(modIds);
        }
    }
}