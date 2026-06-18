using AmaxApiAdapter.Models.DTOs;
using racebookApi.Models;

namespace racebookApi.Repositories.Interfaces
{
    public interface IPlayerStatsSnapshotRepository
    {
        Task<Guid> InsertSnapshot(PlayerStats playerStats);
        Task<PlayerStatsSnapshot> GetPlayerStatsSnapshotByIdAsync(string snapshotId);
    }
}