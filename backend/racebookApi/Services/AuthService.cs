using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using racebookApi.Models;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace racebookApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IHttpClientFactory httpClientFactory, IUserRepository userRepository, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("amax-api");
            _userRepository = userRepository;
            _configuration = configuration;
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

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            if (!await _userRepository.UserExists(dto.Email))
            {
                return null;
            }

            AccountInfo accountInfo = await _userRepository.GetAccountInfoByEmail(dto.Email);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, accountInfo.PasswordHash))
            {
                return null;
            }

            return GenerateToken(accountInfo);
        }

        private string GenerateToken(AccountInfo accountInfo)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountInfo.Uid.ToString()),
                new Claim(ClaimTypes.Name, accountInfo.Username),
                new Claim(ClaimTypes.GivenName, accountInfo.AmaxUsername ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"]!,
                audience: _configuration["JwtSettings:Audience"]!,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
