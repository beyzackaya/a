using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatAppDbContext _context;

        public MessageRepository(ChatAppDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.GroupUser)
                .FirstOrDefaultAsync(m => m.MessageId == id);
        }

        public async Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(int groupId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.GroupUser)
                .Where(m => m.GroupId == groupId && !m.isDeleted)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.SenderId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}