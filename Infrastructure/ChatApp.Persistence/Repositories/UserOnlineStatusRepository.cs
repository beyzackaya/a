using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class UserOnlineStatusRepository : IUserOnlineStatusRepository
    {
        private readonly ChatAppDbContext _context;

        public UserOnlineStatusRepository(ChatAppDbContext context)
        {
            _context = context;
        }

        public async Task<UserOnlineStatus?> GetLatestStatusByUserIdAsync(int userId)
        {
            return await _context.UserOnlineStatuses
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.LastSeen)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(UserOnlineStatus status)
        {
            await _context.UserOnlineStatuses.AddAsync(status);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserOnlineStatus status)
        {
            _context.UserOnlineStatuses.Update(status);
            await _context.SaveChangesAsync();
        }
    }
}