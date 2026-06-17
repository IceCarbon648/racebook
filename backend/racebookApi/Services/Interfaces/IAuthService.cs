using racebookApi.Models.DTOs.FromClient;
using System.Text.Json;

namespace racebookApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JsonDocument> GetUserAmaxData(HttpContext httpContext);
        bool HasAmaxAccount(JsonDocument userAmaxData);
        string GetAmaxUsername(JsonDocument userAmaxData);
        Task setAmaxUsername(string playerName);
        Task<string?> LoginAsync(LoginDto dto);
    }
}