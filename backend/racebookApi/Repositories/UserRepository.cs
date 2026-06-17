using Dapper;
using racebookApi.Models;
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

        public async Task<AccountInfo> GetAccountInfoByEmail(string email)
        {
            string sql = @"SELECT
                               Uid,
                               Username,
                               AmaxUsername,
                               PasswordHash
                           FROM [User]
                           WHERE Email = @email";

            return await _dbConnection.QueryFirstAsync<AccountInfo>(sql, new { email });
        }

        public async Task<bool> UserExists(string email)
        {
            string sql = @"SELECT COUNT(1)
                           FROM [User]
                           WHERE Email = @email";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { email });
        }

        public async Task RegisterUser(string email, string username, string passwordHash)
        {
            string sql = @"INSERT INTO [User] (
                               Email,
                               Username,
                               PasswordHash
                           )
                           VALUES (
                               @email,
                               @username,
                               @passwordHash
                           )";

            await _dbConnection.ExecuteAsync(sql, new { email, username, passwordHash });
        }
    }
}