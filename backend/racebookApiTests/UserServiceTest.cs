using Infrastructure.Interfaces;
using NSubstitute;
using Business;
using Business.Models.DTOs.Request;
using Microsoft.Extensions.Logging;

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
    public async Task GivenNewEmail_WhenRegisterUserAsyncIsCalled_ThenUserIsRegistered()
    {
        //Arrange
        RegisterUserDto dto = new RegisterUserDto
        {
            Email = "testing@test.com",
            Username = "TestUser",
            Password = "Password1!"
        };

        _userRepository.RegisterUser(dto.Email, dto.Username, Arg.Any<string>()).Returns(true);

        //Act
        bool result = await _userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(result, Is.True);
        await _userRepository.Received(1).RegisterUser(dto.Email, dto.Username, Arg.Any<string>());
    }

    [Test]
    public async Task GivenExistingEmail_WhenRegisterUserAsyncIsCalled_ThenUserIsNotRegistered()
    {
        //Arrange
        RegisterUserDto dto = new RegisterUserDto
        {
            Email = "existing.mail@test.com",
            Username = "TestUser",
            Password = "Password1!"
        };

        _userRepository.RegisterUser(dto.Email, dto.Username, Arg.Any<string>()).Returns(false);

        //Act
        bool result = await _userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(result, Is.False);
        await _userRepository.Received(1).RegisterUser(dto.Email, dto.Username, Arg.Any<string>());
    }
}