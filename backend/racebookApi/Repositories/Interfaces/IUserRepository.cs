namespace racebookApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<string> GetUsernameByUserId(string userId);
        Task UpdateAmaxUsername(string playerName);
        Task GetAccountInfoByEmail(string email);
    }
}