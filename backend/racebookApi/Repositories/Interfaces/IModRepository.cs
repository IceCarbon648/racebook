using racebookApi.Models;
using racebookApi.Models.DTOs.ToClient;

namespace racebookApi.Repositories.Interfaces
{
    public interface IModRepository
    {
        Task<Guid> CreateMod(string uid, string title, string type, string description, string uploadDate, string editDate, string modFileUrl);
        Task DeleteMod(string modId);
        Task<string> GetModFileUrl(string modId);
        Task<Mod> GetModById(string modId);
        Task EditMod(Mod mod);
        Task<List<string>> GetAllModIds();
    }
}