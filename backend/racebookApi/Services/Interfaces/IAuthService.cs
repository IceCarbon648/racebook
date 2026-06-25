using racebookApi.Models.DTOs.FromClient;

namespace racebookApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
    }
}