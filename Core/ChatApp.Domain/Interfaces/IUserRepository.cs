using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> SearchByUsernameAsync(string searchTerm);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task DeleteAsync(User user);
    }
}