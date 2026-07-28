using Business;
using Business.Models.DTOs.Request;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace racebookApiTests;

[TestFixture]
public class AuthServiceTests
{
    private IUserRepository _userRepository;
    private IConfiguration _configuration;
    private ILogger<AuthService> _logger;
    private AuthService _authService;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<AuthService>>();

        Dictionary<string, string?> configValues = new()
        {
            { "JwtSettings:Key", "this-is-a-test-key-that-is-long-enough-for-hmac" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _authService = new AuthService(_userRepository, _configuration, _logger);
    }

    [Test]
    public async Task GivenValidCredentials_WhenLoginAsyncIsCalled_ThenJwtIsReturned()
    {
        //Arrange
        LoginDto dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password1!"
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = Guid.NewGuid(),
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task GivenNonExistentEmail_WhenLoginAsyncIsCalled_ThenNullIsReturned()
    {
        //Arrange
        LoginDto dto = new LoginDto
        {
            Email = "nonexistent.mail@test.com",
            Password = "Password1!"
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns((AccountInfo?)null);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Null);
        await _userRepository.Received(1).GetAccountInfoByEmail(dto.Email);
    }

    [Test]
    public async Task GivenInvalidPassword_WhenLoginAsyncIsCalled_ThenNullIsReturned()
    {
        //Arrange
        LoginDto dto = new LoginDto
        {
            Email = "testing@test.com",
            Password = "WrongPassword1!"
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = Guid.NewGuid(),
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Null);
        await _userRepository.Received(1).GetAccountInfoByEmail(dto.Email);
    }

    [Test]
    public void GivenValidDiscordUser_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenJwtIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string username = "TestUser";
        string amaxUsername = "AmaxUser";

        //Act
        string result = _authService.GenerateTokenWithAmaxUsername(uid, username, amaxUsername);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Split('.'), Has.Length.EqualTo(3));
    }

    [Test]
    public void GivenValidDiscordUser_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenTokenContainsCorrectClaims()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string username = "TestUser";
        string amaxUsername = "AmaxUser";

        //Act
        string result = _authService.GenerateTokenWithAmaxUsername(uid, username, amaxUsername);

        //Assert
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.ReadJwtToken(result);

        Assert.That(token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, Is.EqualTo(uid));
        Assert.That(token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value, Is.EqualTo(username));
        Assert.That(token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value, Is.EqualTo(amaxUsername));
    }
}