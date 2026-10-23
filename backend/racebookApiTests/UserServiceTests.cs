using Business;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Models.DTOs.Request;
using NSubstitute;

namespace racebookApiTests;

[TestFixture]
public class UserServiceTests
{
    private IUserRepository _userRepository;
    private ILogger<UserService> _logger;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UserService>>();
        _userService = new UserService(_userRepository, _logger);
    }

    [Test]
    public async Task GivenValidInput_WhenRegisterUserAsyncIsCalled_ThenUserIsRegisteredAndTrueIsReturned()
    {
        //Arrange
        RegisterUserDto dto = new RegisterUserDto
        {
            Email = "test@example.com",
            Username = "TestUser",
            Password = "TestPassword123!"
        };

        _userRepository.RegisterUser(dto.Email, dto.Username, Arg.Any<string>()).Returns(true);

        //Act
        bool result = await _userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(result, Is.True);
        await _userRepository.Received(1).RegisterUser(dto.Email, dto.Username, Arg.Any<string>());
    }

    [Test]
    public async Task GivenValidInput_WhenRegisterUserAsyncIsCalled_ThenPasswordIsHashedBeforeBeingStored()
    {
        //Arrange
        string storedPassword = string.Empty;

        RegisterUserDto dto = new RegisterUserDto
        {
            Email = "test@example.com",
            Username = "TestUser",
            Password = "TestPassword123!"
        };

        _userRepository.RegisterUser(
            dto.Email,
            dto.Username,
            Arg.Do<string>(password => storedPassword = password))
            .Returns(true);

        //Act
        await _userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(storedPassword, Is.Not.Empty);
        Assert.That(storedPassword, Is.Not.EqualTo(dto.Password));
        Assert.That(BCrypt.Net.BCrypt.Verify(dto.Password, storedPassword), Is.True);
    }

    [Test]
    public async Task GivenTheSamePassword_WhenRegisterUserAsyncIsCalledTwice_ThenDifferentHashesAreStored()
    {
        //Arrange
        List<string> storedPasswords = [];

        RegisterUserDto firstDto = new RegisterUserDto
        {
            Email = "first@example.com",
            Username = "FirstUser",
            Password = "TestPassword123!"
        };

        RegisterUserDto secondDto = new RegisterUserDto
        {
            Email = "second@example.com",
            Username = "SecondUser",
            Password = "TestPassword123!"
        };

        _userRepository.RegisterUser(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Do<string>(password => storedPasswords.Add(password)))
            .Returns(true);

        //Act
        await _userService.RegisterUserAsync(firstDto);
        await _userService.RegisterUserAsync(secondDto);

        //Assert
        Assert.That(storedPasswords, Has.Count.EqualTo(2));
        Assert.That(storedPasswords[0], Is.Not.EqualTo(storedPasswords[1]));
    }

    [Test]
    public async Task GivenEmailAlreadyInUse_WhenRegisterUserAsyncIsCalled_ThenFalseIsReturned()
    {
        //Arrange
        RegisterUserDto dto = new RegisterUserDto
        {
            Email = "taken@example.com",
            Username = "TestUser",
            Password = "TestPassword123!"
        };

        _userRepository.RegisterUser(dto.Email, dto.Username, Arg.Any<string>()).Returns(false);

        //Act
        bool result = await _userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(result, Is.False);
        await _userRepository.Received(1).RegisterUser(dto.Email, dto.Username, Arg.Any<string>());
    }
}
