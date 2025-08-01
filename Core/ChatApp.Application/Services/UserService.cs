using ChatApp.Application.DTOs.UserDTOs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user is null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<IEnumerable<UserDto>> SearchUsersByUsernameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<UserDto>();
            }

            var users = await _userRepository.SearchByUsernameAsync(searchTerm);
            return users.Select(MapToDto);
        }

        public async Task CreateUserAsync(CreateUserDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                LastName = userDto.LastName,
                UserName = userDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.UserId);
            if (user == null) return;

            user.Name = userDto.Name;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;

            await _userRepository.UpdateAsync(user);
        }
        public async Task UpdateUserByUsernameAsync(string username, UpdateUserByUsernameDto userDto)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return;

            user.Name = userDto.Name;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Email = userDto.Email;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
        public async Task DeleteUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user.UserId);
            }
        }

        private static UserDto MapToDto(User user) => new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}