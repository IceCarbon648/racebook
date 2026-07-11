using System.Text.Json;

namespace AmaxApiAdapter.Http
{
    public interface IAmaxHttpClient
    {
        Task<JsonDocument> GetUserAmaxData(string bearerToken);
        Task<JsonDocument> GetPlayerStats(string amaxUsername);
    }
}