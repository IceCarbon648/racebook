using Infrastructure.Interfaces;
using System.Data;
using Dapper;

namespace Infrastructure
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnection _dbConnection;

        public CategoryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<string>> GetCategories()
        {
            string sql = @"SELECT Name
                           FROM Category
                           ORDER BY DisplayOrder";

            IEnumerable<string> queryResult = await _dbConnection.QueryAsync<string>(sql);

            return queryResult.ToList();
        }
    }
}