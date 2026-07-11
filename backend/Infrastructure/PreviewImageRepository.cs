using Infrastructure.Interfaces;
using System.Data;
using Dapper;

namespace Infrastructure
{
    public class PreviewImageRepository : IPreviewImageRepository
    {
        private readonly IDbConnection _dbConnection;

        public PreviewImageRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task CreatePreviewImage(Guid modId, string imageUrl)
        {
            string sql = @"INSERT INTO PreviewImage (
                               ModId,
                               FilePath
                           )
                           VALUES (
                               @modId,
                               @imageUrl
                           )";

            await _dbConnection.ExecuteAsync(sql, new { modId, imageUrl });
        }

        public async Task DeletePreviewImageByModId(string modId)
        {
            string sql = @"DELETE
                           FROM PreviewImage
                           WHERE ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new { modId });
        }

        public async Task<List<string>> GetPreviewImageUrl(string modId)
        {
            string sql = @"SELECT FilePath
                           FROM PreviewImage
                           WHERE ModId = @modId";

            IEnumerable<string> queryResult = await _dbConnection.QueryAsync<string>(sql, new { modId });

            return queryResult.ToList();
        }

        public async Task DeletePreviewImageByUrl(string url)
        {
            string sql = @"DELETE
                           FROM PreviewImage
                           WHERE FilePath = @url";

            await _dbConnection.ExecuteAsync(sql, new { url });
        }
    }
}