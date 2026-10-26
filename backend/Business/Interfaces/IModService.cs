using Models.DTOs.Request;
using Models;
using Models.DTOs.Response;

namespace Business.Interfaces
{
    public interface IModService
    {
        Task<Guid?> UploadMod(string uid, ModDto dto);
        Task DeleteMod(string modId);
        Task<bool> EditMod(string modId, ModEditDto dto);
        Task<List<GetModDto>> GetAllMods(string? uid);
        Task<List<Mod>> GetMyMods(string uid);
        Task<bool> IsOwner(string modId, string uid);
    }
}