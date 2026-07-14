using Infrastructure.Models.DTOs.Response;
using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IModRepository
    {
        Task<Guid> CreateMod(string uid, string title, string type, string description, string uploadDate, string editDate, string modFileUrl, string previewImageUrl);
        Task<Mod> DeleteMod(string modId);
        Task<string> GetModFileUrl(string modId);
        Task<Mod> GetModById(string modId);
        Task EditMod(Mod mod, string? title, string? type, string? description);
        Task<List<GetModDto>> GetAllMods();
        Task<List<Mod>> GetMyMods(string uid);
    }
}