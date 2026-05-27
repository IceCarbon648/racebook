using racebookApi.Constants;
using racebookApi.Models.DTOs.FromClient;

namespace racebookApi.Services.Interfaces
{
    public interface IModService
    {
        Task UploadMod(ModDto dto);
        Task DeleteMod(string modId);
        Task EditMod(ModEditDto dto);
    }
}