using Dapper;
using Infrastructure.Models;
using Infrastructure.Interfaces;
using System.Data;

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

        public async Task EditMod(Mod mod)
        {
            string sql = @"UPDATE Mod
                           SET Title = @title,
                               Type = @type,
                               Description = @description,
                               EditDate = @editDate,
                               ModFileUrl = @modFileUrl,
                               ImageUrl = @imageUrl
                           WHERE ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new {
                modId = mod.ModId,
                title = mod.Title,
                type = mod.Type,
                description = mod.Description,
                editDate = mod.EditDate,
                modFileUrl = mod.ModFileUrl,
                imageUrl = mod.ImageUrl,
            });
        }

        public async Task<List<Guid>> GetAllModIds()
        {
            string sql = @"SELECT ModId
                           FROM Mod";

            IEnumerable<Guid> queryResult = await _dbConnection.QueryAsync<Guid>(sql);

            return queryResult.ToList();
        }

        public async Task<List<Guid>> GetMyModIds(string uid)
        {
            string sql = @"SELECT ModId
                           FROM Mod
                           WHERE Uid = @uid";

            IEnumerable<Guid> queryResult = await _dbConnection.QueryAsync<Guid>(sql, new { uid });

            return queryResult.ToList();
        }
    }
}