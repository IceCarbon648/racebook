using Models.DTOs.Request;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
        Task SetAmaxUsername(string uid, string playerName);
    }
}