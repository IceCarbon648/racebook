using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Business;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Request;
using NSubstitute;

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

        // A real configuration rather than a substitute: GenerateToken may read its
        // settings through GetSection or GetValue, which are extension methods and
        // cannot be stubbed. The key must be at least 32 characters or
        // SymmetricSecurityKey rejects it.
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-signing-key-that-is-long-enough-for-hmac-sha256",
                ["Jwt:Issuer"] = "racebook-tests",
                ["Jwt:Audience"] = "racebook-tests",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        _authService = new AuthService(_userRepository, _configuration, _logger);
    }

    [Test]
    public async Task GivenNonExistentEmail_WhenLoginAsyncIsCalled_ThenNullIsReturned()
    {
        //Arrange
        LoginDto dto = new LoginDto
        {
            Email = "missing@example.com",
            Password = "TestPassword123!"
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns((AccountInfo?)null);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenIncorrectPassword_WhenLoginAsyncIsCalled_ThenNullIsReturned()
    {
        //Arrange
        LoginDto dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = Guid.NewGuid(),
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!")
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenValidCredentials_WhenLoginAsyncIsCalled_ThenTokenIsReturned()
    {
        //Arrange
        string password = "TestPassword123!";

        LoginDto dto = new LoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = Guid.NewGuid(),
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task GivenValidCredentials_WhenLoginAsyncIsCalled_ThenTokenContainsTheUsersId()
    {
        //Arrange
        string password = "TestPassword123!";
        Guid uid = Guid.NewGuid();

        LoginDto dto = new LoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = uid,
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(result);

        Assert.That(
            token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            Is.EqualTo(uid.ToString()));
    }

    [Test]
    public async Task GivenValidCredentials_WhenLoginAsyncIsCalled_ThenTokenDoesNotContainThePasswordHash()
    {
        //Arrange
        string password = "TestPassword123!";
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        LoginDto dto = new LoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        AccountInfo accountInfo = new AccountInfo
        {
            Uid = Guid.NewGuid(),
            Username = "TestUser",
            AmaxUsername = null,
            PasswordHash = passwordHash
        };

        _userRepository.GetAccountInfoByEmail(dto.Email).Returns(accountInfo);

        //Act
        string? result = await _authService.LoginAsync(dto);

        //Assert
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(result);

        Assert.That(token.Claims.Any(c => c.Value == passwordHash), Is.False);
        Assert.That(token.Claims.Any(c => c.Value == password), Is.False);
    }

    [Test]
    public void GivenValidInput_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenTokenIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act
        string result = _authService.GenerateTokenWithAmaxUsername(uid, username, amaxUsername);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void GivenValidInput_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenTokenContainsTheAmaxUsername()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act
        string result = _authService.GenerateTokenWithAmaxUsername(uid, username, amaxUsername);

        //Assert
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(result);

        Assert.That(
            token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            Is.EqualTo(uid));
        Assert.That(token.Claims.Any(c => c.Value == amaxUsername), Is.True);
    }

    [Test]
    public void GivenNoAmaxUsername_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenTokenIsStillReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string username = "TestUser";

        //Act
        string result = _authService.GenerateTokenWithAmaxUsername(uid, username, null!);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void GivenEmptyUid_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenArgumentExceptionIsThrown()
    {
        //Arrange
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<ArgumentException>(
            () => _authService.GenerateTokenWithAmaxUsername(string.Empty, username, amaxUsername));
    }

    [Test]
    public void GivenWhitespaceUid_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenArgumentExceptionIsThrown()
    {
        //Arrange
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<ArgumentException>(
            () => _authService.GenerateTokenWithAmaxUsername("   ", username, amaxUsername));
    }

    [Test]
    public void GivenNullUid_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenArgumentNullExceptionIsThrown()
    {
        //Arrange
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _authService.GenerateTokenWithAmaxUsername(null!, username, amaxUsername));
    }

    [Test]
    public void GivenEmptyUsername_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenArgumentExceptionIsThrown()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<ArgumentException>(
            () => _authService.GenerateTokenWithAmaxUsername(uid, string.Empty, amaxUsername));
    }

    [Test]
    public void GivenNullUsername_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenArgumentNullExceptionIsThrown()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _authService.GenerateTokenWithAmaxUsername(uid, null!, amaxUsername));
    }

    [Test]
    public void GivenAnInvalidUid_WhenGenerateTokenWithAmaxUsernameIsCalled_ThenFormatExceptionIsThrown()
    {
        //Arrange
        string username = "TestUser";
        string amaxUsername = "AmaxTestUser";

        //Act & Assert
        Assert.Throws<FormatException>(
            () => _authService.GenerateTokenWithAmaxUsername("not-a-guid", username, amaxUsername));
    }
}
