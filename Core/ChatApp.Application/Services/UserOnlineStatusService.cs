using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class UserOnlineStatusService : IUserOnlineStatusService
    {
        private readonly IUserOnlineStatusRepository _repository;

        public UserOnlineStatusService(IUserOnlineStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserOnlineStatus?> GetLatestStatusAsync(int userId)
        {
            return await _repository.GetLatestStatusByUserIdAsync(userId);
        }

        public async Task SetOnlineStatusAsync(int userId, bool isOnline)
        {
            var status = new UserOnlineStatus
            {
                UserId = userId,
                IsOnline = isOnline,
                LastSeen = DateTime.UtcNow
            };

            await _repository.AddAsync(status);
        }
    }
}