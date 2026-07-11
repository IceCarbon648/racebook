using Business.Models.DTOs.Request;
using Business.Models.DTOs.Response;

namespace Business.Interfaces
{
    public interface IModService
    {
        Task UploadMod(string uid, ModDto dto);
        Task DeleteMod(string modId);
        Task EditMod(string modId, ModEditDto dto);
        Task<GetModDto> GetMod(string modId);
        Task<List<GetModDto>> GetAllMods();
        Task<List<GetModDto>> GetMyMods(string uid);
    }
}