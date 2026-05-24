using racebookApi.Data;
using racebookApi.Repositories.Interfaces;
using System.Data;
using Dapper;

namespace racebookApi.Repositories
{
    public class PreviewImageRepository : IPreviewImageRepository
    {
        private readonly IDbConnection _dbConnection;

        public PreviewImageRepository(IDapperContext dapperContext)
            => _dbConnection = dapperContext.CreateConnection();

        public async Task CreatePreviewImage(Guid modId, string imageUrl)
        {
            string sql = "INSERT INTO PreviewImage (ModId, FilePath) VALUES (@modId, @imageUrl)";

            await _dbConnection.ExecuteAsync(sql, new { modId, imageUrl });
        }
    }
}