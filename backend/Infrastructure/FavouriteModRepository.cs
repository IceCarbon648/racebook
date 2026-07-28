using Dapper;
using Infrastructure.Interfaces;
using System.Data;
using Models.DTOs.Response;

namespace Infrastructure
{
    public class FavouriteModRepository : IFavouriteModRepository
    {
        private readonly IDbConnection _dbConnection;

        public FavouriteModRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task AddToFavourites(string uid, string modId)
        {
            string sql = @"INSERT INTO FavouriteMod (
                               Uid,
                               ModId
                           )
                           VALUES (
                               @uid,
                               @modId
                           )";

            await _dbConnection.ExecuteAsync(sql, new { uid, modId });
        }

        public async Task<List<GetModDto>> GetFavourites(string uid)
        {
            string sql = @"SELECT
                               u.Username
                                   AS 'Creator',
                               m.Title,
                               m.Type,
                               m.Description,
                               m.UploadDate,
                               m.EditDate,
                               m.ModFileUrl,
                               m.ImageUrl
                                   AS 'PreviewImageUrl'
                           FROM FavouriteMod fm
                           INNER JOIN Mod m
                               ON fm.ModId = m.ModId
                           INNER JOIN [User] u
                               ON m.Uid = u.Uid
                           WHERE fm.Uid = @uid";

            IEnumerable<GetModDto> queryResult = await _dbConnection.QueryAsync<GetModDto>(sql, new { uid });

            return queryResult.ToList();
        }

        public async Task DeleteFromFavourites(string uid, string modId)
        {
            string sql = @"DELETE FROM FavouriteMod
                           WHERE
                               Uid = @uid
                           AND
                               ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new { uid, modId });
        }

        public async Task DeleteFavouriteModReference(string modId)
        {
            string sql = @"DELETE FROM FavouriteMod
                           WHERE ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new { modId });
        }
    }
}