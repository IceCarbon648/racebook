using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AmaxApiAdapter.Http
{
    public class AmaxHttpClient : IAmaxHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AmaxHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<JsonDocument> GetUserAmaxData(string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            string url = $"{_configuration["AmaxApi:BaseUrl"]}players/@me";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            JsonDocument jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            return jsonResponse;
        }

        public async Task<JsonDocument> GetPlayerStats(string amaxUsername)
        {
            string url = $"{_configuration["AmaxApi:BaseUrl"]}players/name/{amaxUsername}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            JsonDocument jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            return jsonResponse;
        }
    }
}