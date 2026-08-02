using Models;

namespace Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task UpdateAmaxUsername(string uid, string amaxUsername);
        Task<AccountInfo?> GetAccountInfoByEmail(string email);
        Task<bool> RegisterUser(string email, string username, string passwordHash);
    }
}