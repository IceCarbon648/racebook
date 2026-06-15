using AmaxApiAdapter.Models.DTOs;

namespace AmaxApiAdapter.Adapters
{
    public interface IAmaxAdapter
    {
        Task<string> GetAmaxUsername(string bearerToken);
        Task<PlayerStats> GetPlayerStats(string amaxUsername);
    }
}