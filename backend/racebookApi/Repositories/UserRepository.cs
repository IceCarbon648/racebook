using Dapper;
using racebookApi.Repositories.Interfaces;
using System.Data;

namespace racebookApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> GetUsernameByUserId(string userId)
        {
            string sql = @"SELECT Username
                           FROM [User]
                           WHERE Uid = @userId";

            return await _dbConnection.ExecuteScalarAsync<string>(sql, new { userId });
        }

        public async Task UpdateAmaxUsername(string playerName)
        {
            string sql = @"UPDATE [User]
                           SET AmaxUsername = @amaxUsername
                           WHERE Username = @username";

            await _dbConnection.ExecuteAsync(sql, new { amaxUsername = playerName, username = "BobBuilder" });
        }

        public async Task GetAccountInfoByEmail(string email)
        {
            string sql = @"SELECT
                               Uid,
                               Username,
                               AmaxUsername
                           FROM [User]
                           WHERE Email = @email";


        }
    }
}