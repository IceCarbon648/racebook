using racebookApi.Models.DTOs.FromClient;

namespace racebookApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
    }
}