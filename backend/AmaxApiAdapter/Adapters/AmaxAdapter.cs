using AmaxApiAdapter.Http;
using AmaxApiAdapter.Models;
using AmaxApiAdapter.Models.DTOs;
using System.Text.Json;

namespace AmaxApiAdapter.Adapters
{
    public class AmaxAdapter : IAmaxAdapter
    {
        private readonly IAmaxHttpClient _amaxHttpClient;

        public AmaxAdapter(IAmaxHttpClient amaxHttpClient)
        {
            _amaxHttpClient = amaxHttpClient;
        }

        private bool HasAmaxAccount(JsonDocument userAmaxData)
        {
            return userAmaxData.RootElement.GetProperty("amax_account").GetBoolean();
        }

        public async Task<string> GetAmaxUsername(string bearerToken)
        {
            JsonDocument userAmaxData = await _amaxHttpClient.GetUserAmaxData(bearerToken);

            if (!HasAmaxAccount(userAmaxData))
            {
                return null!;
            }

            return userAmaxData.RootElement
                .GetProperty("amax_player_data")
                .GetProperty("stats")
                .GetProperty("playerName")
                .GetString()!;
        }

        public async Task<PlayerStats> GetPlayerStats(string amaxUsername)
        {
            JsonDocument publicPlayerData = await _amaxHttpClient.GetPlayerStats(amaxUsername);

            AmaxStatsData amaxStatsData = publicPlayerData.RootElement
                .GetProperty("data")
                .GetProperty("amaxPlayerData")
                .GetProperty("amaxStatsData")
                .Deserialize<AmaxStatsData>()!;

            PlayerStats playerStats = new PlayerStats
            {
                TotalFans = amaxStatsData.StatFans,
                DriverScore = amaxStatsData.StatDriverScore,
                RaceTime = amaxStatsData.StatRaceTime,
                RaceStarts = amaxStatsData.StatRaces,
                Wins = amaxStatsData.StatFirst,
                PodiumFinishes = amaxStatsData.StatTopThree,
                PowerUpUses = amaxStatsData.StatFired,
                PowerUpHits = amaxStatsData.StatHits
            };

            return playerStats;
        }
    }
}