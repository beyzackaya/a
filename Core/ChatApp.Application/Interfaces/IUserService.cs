using ChatApp.Application.DTOs.UserDTOs;

namespace ChatApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> SearchUsersByUsernameAsync(string searchTerm);
        Task UpdateUserAsync(UpdateUserDto userDto);
        Task DeleteUserByIdAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
        Task UpdateUserByUsernameAsync(string username, UpdateUserByUsernameDto dto);
        Task CreateUserAsync(CreateUserDto userDto);
    }
}