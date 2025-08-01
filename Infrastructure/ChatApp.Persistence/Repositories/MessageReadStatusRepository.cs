using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class MessageReadStatusRepository : IMessageReadStatusRepository
    {
        private readonly ChatAppDbContext _context;

        public MessageReadStatusRepository(ChatAppDbContext context)
        {
            _context = context;
        }

        public async Task<MessageReadStatus?> GetByIdAsync(int id)
        {
            return await _context.MessageReadStatuses.FindAsync(id);
        }

        public async Task<IEnumerable<MessageReadStatus>> GetByMessageIdAsync(int messageId)
        {
            return await _context.MessageReadStatuses
                .Where(mrs => mrs.MessageId == messageId)
                .ToListAsync();
        }

        public async Task AddAsync(MessageReadStatus status)
        {
            await _context.MessageReadStatuses.AddAsync(status);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MessageReadStatus status)
        {
            _context.MessageReadStatuses.Update(status);
            await _context.SaveChangesAsync();
        }
    }
}