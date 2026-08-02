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

        public async Task<Guid> CreateMod(string uid, string title, string type, string description, string uploadDate, string editDate, string modFileUrl, string previewImageUrl)
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

            return await _dbConnection.ExecuteScalarAsync<Guid>(sql, new { uid, title, type, description, uploadDate, editDate, modFileUrl, previewImageUrl });
        }

        public async Task<Mod> DeleteMod(string modId)
        {
            string sql = @"DELETE 
                           FROM Mod
                           OUTPUT DELETED.*
                           WHERE ModId = @modId";

            return await _dbConnection.QueryFirstOrDefaultAsync<Mod>(sql, new { modId });
        }

        public async Task<string> GetModFileUrl(string modId)
        {
            string sql = @"SELECT FilePath
                           FROM Mod
                           WHERE ModId = @modId";

            return await _dbConnection.ExecuteScalarAsync<string>(sql, new { modId });
        }

        public async Task<Mod> GetModById(string modId)
        {
            string sql = @"SELECT *
                           FROM Mod
                           WHERE ModId = @modId";

            return await _dbConnection.QueryFirstAsync<Mod>(sql, new { modId });
        }

        public async Task EditMod(Mod mod, string? title, string? type, string? description)
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
                title,
                type,
                description,
                editDate = mod.EditDate,
                modFileUrl = mod.ModFileUrl,
                imageUrl = mod.ImageUrl,
            });
        }

        public async Task<List<GetModDto>> GetAllMods()
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
                           FROM Mod m
                           INNER JOIN [User] u ON m.Uid = u.Uid";

            IEnumerable<GetModDto> queryResult = await _dbConnection.QueryAsync<GetModDto>(sql);

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
    }
}