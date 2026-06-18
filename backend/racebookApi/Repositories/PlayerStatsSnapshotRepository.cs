using AmaxApiAdapter.Models.DTOs;
using Dapper;
using racebookApi.Models;
using racebookApi.Repositories.Interfaces;
using System.Data;

namespace racebookApi.Repositories
{
    public class PlayerStatsSnapshotRepository : IPlayerStatsSnapshotRepository
    {
        private readonly IDbConnection _dbConnection;

        public PlayerStatsSnapshotRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> InsertSnapshot(PlayerStats playerStats)
        {
            string sql = @"INSERT INTO PlayerStatsSnapshot (
                               TotalFans,
                               TotalRaceTimeMilleconds,
                               DriverScore,
                               RaceStarts,
                               RaceWins,
                               RacePodiums,
                               PowerUpUses,
                               PowerUpHits,
                               Date
                           )
                           OUTPUT INSERTED.SnapshotId
                           VALUES (
                               @totalFans,
                               @totalRaceTime,
                               @driverScore,
                               @raceStarts,
                               @raceWins,
                               @racePodiums,
                               @powerUpUses,
                               @powerUpHits,
                               @date
                           )";

            return await _dbConnection.ExecuteScalarAsync<Guid>(sql, new
            {
                totalFans = playerStats.TotalFans,
                totalRaceTime = playerStats.RaceTime,
                driverScore = playerStats.DriverScore,
                raceStarts = playerStats.RaceStarts,
                raceWins = playerStats.Wins,
                racePodiums = playerStats.PodiumFinishes,
                powerUpUses = playerStats.PowerUpUses,
                powerUpHits = playerStats.PowerUpHits,
                date = System.DateTime.Now
            });
        }

        public async Task<PlayerStatsSnapshot> GetPlayerStatsSnapshotByIdAsync(string snapshotId)
        {
            string sql = @"SELECT *
                           FROM PlayerStatsSnapshot
                           WHERE SnapshotId = @snapshotId";

            return await _dbConnection.QueryFirstAsync<PlayerStatsSnapshot>(sql, new { SnapshotId = snapshotId });
        }
    }
}
