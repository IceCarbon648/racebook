using Models.DTOs.Response;
using Models;

namespace Infrastructure.Interfaces
{
    public interface IModRepository
    {
        Task<Guid> CreateMod(NewMod newMod);
        Task<Mod?> DeleteMod(string modId);
        Task<Mod?> GetModById(string modId);
        Task EditMod(Mod mod);
        Task<List<GetModDto>> GetAllMods(string? uid);
        Task<List<Mod>> GetMyMods(string uid);
        Task<bool> IsOwner(string modId, string uid);
    }
}