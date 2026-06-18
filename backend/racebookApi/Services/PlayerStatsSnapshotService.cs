using AmaxApiAdapter.Adapters;
using AmaxApiAdapter.Models.DTOs;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;

namespace racebookApi.Services
{
    public class PlayerStatsSnapshotService : IPlayerStatsSnapshotService
    {
        private readonly IAmaxAdapter _amaxAdapter;
        private readonly IPlayerStatsSnapshotRepository _playerStatsSnapshotRepository;

        public PlayerStatsSnapshotService(IAmaxAdapter amaxAdapter, IPlayerStatsSnapshotRepository playerStatsSnapshotRepository)
        {
            _amaxAdapter = amaxAdapter;
            _playerStatsSnapshotRepository = playerStatsSnapshotRepository;
        }

        public async Task SaveSnapshot(string amaxUsername)
        {
            PlayerStats playerStats = await _amaxAdapter.GetPlayerStats(amaxUsername);

            Guid uid = await _playerStatsSnapshotRepository.InsertSnapshot(playerStats);
        }
    }
}
