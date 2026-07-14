using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<string> GetUsernameByUserId(string userId);
        Task UpdateAmaxUsername(string uid, string amaxUsername);
        Task<AccountInfo?> GetAccountInfoByEmail(string email);
        Task<bool> RegisterUser(string email, string username, string passwordHash);
    }
}