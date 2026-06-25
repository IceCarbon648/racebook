using racebookApi.Models.DTOs.FromClient;

namespace racebookApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
        Task setAmaxUsername(string uid, string playerName);
    }
}