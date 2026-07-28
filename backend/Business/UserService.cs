using Models.DTOs.Request;
using Infrastructure.Interfaces;
using Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDto dto)
        {
            _logger.LogInformation("Registering new user with email {Email}", dto.Email);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            bool registeredSuccessfully = await _userRepository.RegisterUser(dto.Email, dto.Username, hashedPassword);

            if (registeredSuccessfully)
            {
                _logger.LogInformation("User successfully registered with email {Email}", dto.Email);
            }
            else
            {
                _logger.LogWarning("Registration failed — email already in use: {Email}", dto.Email);
            }

            return registeredSuccessfully;
        }

        public async Task setAmaxUsername(string uid, string playerName)
        {
            await _userRepository.UpdateAmaxUsername(uid, playerName);
        }
    }
}