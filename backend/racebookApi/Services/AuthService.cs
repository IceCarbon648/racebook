using Microsoft.AspNetCore.Authentication;
using racebookApi.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace racebookApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("amax-api");
        }

        private async Task<string> GetDiscordBearerToken(HttpContext httpContext)
        {
            string discordBearerToken = await httpContext.GetTokenAsync("access_token");

            return discordBearerToken;
        }

        public async Task<JsonDocument> GetUserAmaxData(HttpContext httpContext)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetDiscordBearerToken(httpContext));

            HttpResponseMessage response = await _httpClient.GetAsync("players/@me");
            JsonDocument jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            return jsonResponse;
        }

        public bool HasAmaxAccount(JsonDocument userAmaxData)
        {
            return userAmaxData.RootElement.GetProperty("amax_account").GetBoolean();
        }

        public string GetAmaxUsername(JsonDocument userAmaxData)
        {
            return userAmaxData.RootElement
                .GetProperty("amax_player_data")
                .GetProperty("stats")
                .GetProperty("playerName")
                .GetString();
        }
    }
}