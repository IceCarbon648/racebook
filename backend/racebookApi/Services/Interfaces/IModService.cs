using racebookApi.Models.DTOs.FromClient;
using racebookApi.Models.DTOs.ToClient;

namespace racebookApi.Services.Interfaces
{
    public interface IModService
    {
        Task UploadMod(ModDto dto);
        Task DeleteMod(string modId);
        Task EditMod(ModEditDto dto);
        Task<byte[]?> DownloadModFile(string modFileUrl);
        Task<GetModDto> GetMod(string modID);
    }
}