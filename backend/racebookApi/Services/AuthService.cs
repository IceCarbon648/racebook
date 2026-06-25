using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using racebookApi.Models;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace racebookApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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