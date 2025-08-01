using ChatApp.Application.DTOs.AuthenticationDTOs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _tokenService;
        public AuthService(IUserRepository userRepository, IJwtService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return _tokenService.GenerateToken(user);
        }
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(dto.UserName);
            if (existingUser != null)
            {
                return false;
            }
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = dto.PhoneNumber
            };
            await _userRepository.AddAsync(user);
            return true;

        }

    }
}