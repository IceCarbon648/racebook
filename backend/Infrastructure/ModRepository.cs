using Dapper;
using Models;
using Infrastructure.Interfaces;
using System.Data;
using Models.DTOs.Response;

namespace Infrastructure
{
    public class ModRepository : IModRepository
    {
        private readonly IDbConnection _dbConnection;

        public ModRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateMod(NewMod newMod)
        {
            string sql = @"INSERT INTO Mod (
                               Uid,
                               Title,
                               Type,
                               Description,
                               UploadDate,
                               EditDate,
                               ModFileUrl,
                               ImageUrl
                           )
                           OUTPUT INSERTED.ModId
                           VALUES (
                               @uid,
                               @title,
                               @type,
                               @description,
                               @uploadDate,
                               @editDate,
                               @modFileUrl,
                               @previewImageUrl
                           )";

            return await _dbConnection.ExecuteScalarAsync<Guid>(sql, new { uid = newMod.Uid,
                                                                           title = newMod.Title,
                                                                           type = newMod.Type,
                                                                           description = newMod.Description,
                                                                           uploadDate = newMod.UploadDate,
                                                                           editDate = newMod.EditDate,
                                                                           modFileUrl = newMod.ModFileUrl,
                                                                           previewImageUrl = newMod.ImageUrl
                                                                         });
        }

        public async Task<Mod?> DeleteMod(string modId)
        {
            string sql = @"DELETE 
                           FROM Mod
                           OUTPUT DELETED.*
                           WHERE ModId = @modId";

            return await _dbConnection.QueryFirstOrDefaultAsync<Mod>(sql, new { modId });
        }

        public async Task<Mod?> GetModById(string modId)
        {
            string sql = @"SELECT *
                           FROM Mod
                           WHERE ModId = @modId";

            return await _dbConnection.QueryFirstOrDefaultAsync<Mod>(sql, new { modId });
        }

        public async Task EditMod(Mod mod)
        {
            string sql = @"UPDATE Mod
                           SET 
                               Title = COALESCE(@title, Title),
                               Type = COALESCE(@type, Type),
                               Description = COALESCE(@description, Description),
                               ModFileUrl = COALESCE(@modFileUrl, ModFileUrl),
                               ImageUrl = COALESCE(@imageUrl, ImageUrl),
                               EditDate = @editDate
                           WHERE ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new {
                modId = mod.ModId,
                mod.Title,
                mod.Type,
                mod.Description,
                editDate = mod.EditDate,
                modFileUrl = mod.ModFileUrl,
                imageUrl = mod.ImageUrl,
            });
        }

        public async Task<List<GetModDto>> GetAllMods(string? uid)
        {
            string sql = @"SELECT
                             m.ModId,
                             u.Username AS Creator,
                             m.Title,
                             m.Type,
                             m.Description,
                             m.UploadDate,
                             m.EditDate,
                             m.ModFileUrl,
                             m.ImageUrl AS PreviewImageUrl,
                         CASE 
                             WHEN @uid IS NULL THEN NULL
                             WHEN f.FavouriteId IS NOT NULL THEN CAST(1 AS BIT)
                             ELSE CAST(0 AS BIT)
                         END AS IsFavourite
                         FROM Mod m
                         INNER JOIN [User] u ON m.Uid = u.Uid
                         LEFT JOIN FavouriteMod f ON f.ModId = m.ModId AND f.Uid = @uid";

            IEnumerable<GetModDto> queryResult = await _dbConnection.QueryAsync<GetModDto>(sql, new { uid });

            return queryResult.ToList();
        }

        public async Task<List<Mod>> GetMyMods(string uid)
        {
            string sql = @"SELECT *
                           FROM Mod
                           WHERE Uid = @uid";

            IEnumerable<Mod> queryResult = await _dbConnection.QueryAsync<Mod>(sql, new { uid });

            return queryResult.ToList();
        }

        public async Task<bool> IsOwner(string modId, string uid)
        {
            string sql = @"SELECT COUNT(1)
                   FROM Mod
                   WHERE Uid = @uid
                     AND ModId = @modId";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { uid, modId }) > 0;
        }
    }
}