using Models.DTOs.Request;
using Models;
using Models.DTOs.Response;

namespace Business.Interfaces
{
    public interface IModService
    {
        Task UploadMod(string uid, ModDto dto);
        Task DeleteMod(string modId);
        Task EditMod(string modId, ModEditDto dto);
        Task<List<GetModDto>> GetAllMods();
        Task<List<Mod>> GetMyMods(string uid);
    }
}