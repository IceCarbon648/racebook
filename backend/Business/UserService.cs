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
            if (await _userRepository.GetAccountInfoByEmail(dto.Email) is not null)
            {
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.RegisterUser(dto.Email, dto.Username, hashedPassword);

            return true;
        }

        public async Task setAmaxUsername(string uid, string playerName)
        {
            await _userRepository.UpdateAmaxUsername(uid, playerName);
        }
    }
}