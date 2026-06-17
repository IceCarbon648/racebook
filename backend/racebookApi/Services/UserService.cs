using racebookApi.Models.DTOs.FromClient;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services.Interfaces;

namespace racebookApi.Services
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
            if (await _userRepository.UserExists(dto.Email))
            {
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.RegisterUser(dto.Email, dto.Username, hashedPassword);

            return true;
        }
    }
}