using Infrastructure.Interfaces;
using NSubstitute;
using Business;
using Business.Models.DTOs.Request;

namespace racebookApiTests;

public class UserServiceTest
{
    private IUserRepository _userRepository;

    RegisterUserDto dto = new RegisterUserDto
    {
        Email = "pat@bonana.com",
        Username = "username",
        Password = "12345678"
    };

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
    }

    [Test]
    public async Task GivenAnExistingEmail_WhenRegistering_ReturnFalse()
    {
        //Arrange
        _userRepository.UserExists(dto.Email).Returns(true);
        UserService userService = new UserService(_userRepository);

        //Act
        bool actualResult = await userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(actualResult, Is.False);
    }

    [Test]
    public async Task GivenAnNonExistentEmail_WhenRegistering_ReturnTrue()
    {
        //Arrange
        _userRepository.UserExists(dto.Email).Returns(false);
        UserService userService = new UserService(_userRepository);

        //Act
        bool actualResult = await userService.RegisterUserAsync(dto);

        //Assert
        Assert.That(actualResult, Is.True);
    }
}
