using AmaxApiAdapter.Adapters;
using AmaxApiAdapter.Models.DTOs;
using NSubstitute;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services;

namespace racebookApiTests;

public class PlayerStatsSnapshortServiceTests
{
    private IPlayerStatsSnapshotRepository _playerStatsSnapshotRepository;
    private IAmaxAdapter _amaxAdapter;

    [SetUp]
    public void Setup()
    {
        _playerStatsSnapshotRepository = Substitute.For<IPlayerStatsSnapshotRepository>();
        _amaxAdapter = Substitute.For<IAmaxAdapter>();
    }

    [Test]
    public async Task GivenAmaxUsername_WhenGettingPlayerStats_WritePlayerStatsToDb()
    {
        //Arrange
        string amaxUsername = "Banan";
        Guid snapshotId = Guid.NewGuid();

        PlayerStats playerStats = new PlayerStats
        {
            TotalFans = 6835313,
            DriverScore = 5582,
            RaceTime = 298613072,
            RaceStarts = 1413,
            Wins = 457,
            PodiumFinishes = 1091,
            PowerUpUses = 29421,
            PowerUpHits = 8622
        };

        _amaxAdapter.GetPlayerStats(amaxUsername).Returns(playerStats);
        _playerStatsSnapshotRepository.InsertSnapshot(playerStats).Returns(snapshotId);

        PlayerStatsSnapshotService playerStatsSnapshotService = new PlayerStatsSnapshotService(_amaxAdapter, _playerStatsSnapshotRepository);

        //Act
        await playerStatsSnapshotService.SaveSnapshot(amaxUsername);

        //Assert
        await _amaxAdapter.Received(1).GetPlayerStats(Arg.Any<string>());
        await _playerStatsSnapshotRepository.Received(1).InsertSnapshot(Arg.Any<PlayerStats>());
    }
}
