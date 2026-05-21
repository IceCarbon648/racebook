using Dapper;
using racebookApi.Data;
using racebookApi.Repositories.Interfaces;
using System.Data;

namespace racebookApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDapperContext dapperContext)
            => _dbConnection = dapperContext.CreateConnection();

        public async Task UpdateAmaxUsername(string playerName)
        {
            string sql = "UPDATE [User] SET AmaxUsername = @amaxUsername WHERE Username = @username";

            await _dbConnection.ExecuteAsync(sql, new { amaxUsername = playerName, username = "BobBuilder" });
        }
    }
}