using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces
{
    public interface IUserOnlineStatusRepository
    {
        Task<UserOnlineStatus?> GetLatestStatusByUserIdAsync(int userId);
        Task AddAsync(UserOnlineStatus status);
        Task UpdateAsync(UserOnlineStatus status);
    }
}