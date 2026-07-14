using Business.Models.DTOs.Request;
using Infrastructure.Interfaces;
using Business.Interfaces;

namespace Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDto dto)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            return await _userRepository.RegisterUser(dto.Email, dto.Username, hashedPassword);
        }

        public async Task setAmaxUsername(string uid, string playerName)
        {
            await _userRepository.UpdateAmaxUsername(uid, playerName);
        }
    }
}