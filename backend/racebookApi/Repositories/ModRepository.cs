using Dapper;
using racebookApi.Repositories.Interfaces;
using System.Data;

namespace racebookApi.Repositories
{
    public class ModRepository : IModRepository
    {
        private readonly IDbConnection _dbConnection;

        public ModRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateMod(string uid, string title, string type, string description, string uploadDate, string editDate, string modFileUrl)
        {
            string sql = @"INSERT INTO Mod (
                               Uid,
                               Title,
                               Type,
                               Description,
                               UploadDate,
                               EditDate,
                               FilePath
                           )
                           OUTPUT INSERTED.ModId
                           VALUES (
                               @uid,
                               @title,
                               @type,
                               @description,
                               @uploadDate,
                               @editDate,
                               @filePath
                           )";

            return await _dbConnection.ExecuteScalarAsync<Guid>(sql, new { uid, title, type, description, uploadDate, editDate, filePath = modFileUrl });
        }

        public async Task DeleteMod(string modId)
        {
            string sql = @"DELETE 
                           FROM Mod
                           WHERE ModId = @modId";

            await _dbConnection.ExecuteAsync(sql, new { modId });
        }

        public async Task<string> GetModFileUrl(string modId)
        {
            string sql = @"SELECT FilePath
                           FROM Mod
                           WHERE ModId = @modId";

            return await _dbConnection.ExecuteScalarAsync<string>(sql, new { modId });
        }
    }
}