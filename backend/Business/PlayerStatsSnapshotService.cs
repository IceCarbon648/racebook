using AmaxApiAdapter.Adapters;
using AmaxApiAdapter.Models.DTOs;
using Infrastructure.Interfaces;
using Business.Interfaces;

namespace Business
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

        public async Task<Guid> SaveSnapshot(string amaxUsername)
        {
            PlayerStats playerStats = await _amaxAdapter.GetPlayerStats(amaxUsername);

            return await _playerStatsSnapshotRepository.InsertSnapshot(playerStats);
        }
    }
}