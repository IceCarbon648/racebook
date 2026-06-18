using Dapper;
using racebookApi.Repositories.Interfaces;
using System.Data;

namespace racebookApi.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly IDbConnection _dbConnection;

        public SessionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task OpenSession(string uid, string name, Guid snapshotId)
        {
            string sql = @"INSERT INTO Session (
                               Uid,
                               Name,
                               StartSnapshotId
                           )
                           VALUES (
                               @uid,
                               @name,
                               @snapshotId
                           )";

            await _dbConnection.ExecuteAsync(sql, new
            {
                uid,
                name,
                snapshotId
            });
        }

        public async Task CloseSession(string sessionId, Guid snapshotId)
        {
            string sql = @"UPDATE Session
                           SET EndSnapshotId = @snapshot
                           WHERE SessionId = @sessionId";

            await _dbConnection.ExecuteAsync(sql, new
            {
                snapshotId,
                sessionId
            });
        }
    }
}