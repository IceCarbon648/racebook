using Ardalis.GuardClauses;
using Business.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            AccountInfo? accountInfo = await _userRepository.GetAccountInfoByEmail(dto.Email);

            if (accountInfo is null)
            {
                _logger.LogWarning("Login failed — no account found for email: {Email}", dto.Email);

                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, accountInfo.PasswordHash))
            {
                _logger.LogWarning("Login failed — invalid password for email: {Email}", dto.Email);

                return null;
            }

            _logger.LogInformation("User {Uid} logged in with {Email}", accountInfo.Uid, dto.Email);

            return GenerateToken(accountInfo);
        }

        public string GenerateTokenWithAmaxUsername(string uid, string username, string amaxUsername)
        {
            Guard.Against.NullOrWhiteSpace(uid, nameof(uid));
            Guard.Against.NullOrWhiteSpace(username, nameof(username));

            _logger.LogInformation("Generating refreshed token for user: {Uid}", uid);

            string dummyHash = "$2a$11$XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

            return GenerateToken(new AccountInfo
            {
                Uid = Guid.Parse(uid),
                Username = username,
                AmaxUsername = amaxUsername,
                PasswordHash = dummyHash
            });
        }

        private string GenerateToken(AccountInfo accountInfo)
        {
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountInfo.Uid.ToString()),
                new Claim(ClaimTypes.Name, accountInfo.Username),
                new Claim(ClaimTypes.GivenName, accountInfo.AmaxUsername ?? "")
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"]!,
                audience: _configuration["JwtSettings:Audience"]!,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            _logger.LogDebug("JWT generated for user: {Uid}", accountInfo.Uid);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}