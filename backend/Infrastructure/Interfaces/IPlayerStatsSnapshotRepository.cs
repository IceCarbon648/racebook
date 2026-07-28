using AmaxApiAdapter.Models.DTOs;
using Models;

namespace Infrastructure.Interfaces
{
    public interface IPlayerStatsSnapshotRepository
    {
        Task<Guid> InsertSnapshot(PlayerStats playerStats);
        Task<PlayerStatsSnapshot> GetPlayerStatsSnapshotByIdAsync(string snapshotId);
    }
}