using Business.Models.DTOs.Request;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
        string GenerateTokenWithAmaxUsername(string uid, string username, string amaxUsername);
    }
}