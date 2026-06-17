using racebookApi.Models;

namespace racebookApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<string> GetUsernameByUserId(string userId);
        Task UpdateAmaxUsername(string playerName);
        Task<AccountInfo> GetAccountInfoByEmail(string email);
        Task<bool> UserExists(string email);
        Task RegisterUser(string email, string username, string passwordHash);
    }
}