using Microsoft.AspNetCore.Authentication;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace racebookApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserRepository _userRepository;

        public AuthService(IHttpClientFactory httpClientFactory, IUserRepository userRepository)
        {
            _httpClient = httpClientFactory.CreateClient("amax-api");
            _userRepository = userRepository;
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

        public async Task setAmaxUsername(string playerName)
        {
            await _userRepository.UpdateAmaxUsername(playerName);
        }
    }
}