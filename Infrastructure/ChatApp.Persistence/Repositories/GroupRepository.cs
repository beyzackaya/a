using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ChatAppDbContext _context;

        public GroupRepository(ChatAppDbContext context)
        {
            _context = context;
        }
        // public async Task<Group?> GetByNameAsync(string groupName)
        // {
        //     return await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
        // }
        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _context.Groups.ToListAsync();
        }
        public async Task<IEnumerable<Group>> GetGroupsByNameAsync(string name)
        {
            return await _context.Groups
                .Where(g => g.GroupName == name)
                .OrderBy(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }
        





        
        public async Task<List<Group>> GetUserGroupsWithLastMessagesAsync(int userId)
        {
            return await _context.GroupUsers
                .Where(gu => gu.UserId == userId)
                .Include(gu => gu.Group)
                    .ThenInclude(g => g.GroupUsers)
                .Include(gu => gu.Group)
                    .ThenInclude(g => g.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Select(gu => gu.Group)
                .ToListAsync();
        }

        public async Task<List<Message>> GetGroupMessagesAsync(int groupId)
        {
            return await _context.Messages
                .Where(m => m.GroupId == groupId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        
    }
}