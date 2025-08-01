using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IUserOnlineStatusService
    {
        Task<UserOnlineStatus?> GetLatestStatusAsync(int userId);
        Task SetOnlineStatusAsync(int userId, bool isOnline);
    }
}