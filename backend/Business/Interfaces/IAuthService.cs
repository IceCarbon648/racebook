using Models.DTOs.Request;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
    }
}