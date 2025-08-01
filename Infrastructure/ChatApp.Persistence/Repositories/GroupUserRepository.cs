using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class GroupUserRepository : IGroupUserRepository
    {
        private readonly ChatAppDbContext _context;

        public GroupUserRepository(ChatAppDbContext context)
        {
            _context = context;
        }

        public async Task<GroupUser?> GetByIdAsync(int id)
        {
            return await _context.GroupUsers.FindAsync(id);
        }

        public async Task<IEnumerable<GroupUser>> GetUsersByGroupIdAsync(int groupId)
        {
            return await _context.GroupUsers
                .Where(gu => gu.GroupId == groupId)
                .Include(gu => gu.User)
                .ToListAsync();
        }

        public async Task AddAsync(GroupUser groupUser)
        {
            await _context.GroupUsers.AddAsync(groupUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gu = await _context.GroupUsers.FindAsync(id);
            if (gu != null)
            {
                _context.GroupUsers.Remove(gu);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<GroupUser?> GetByGroupIdAndUserIdAsync(int groupId, int userId)
        {
            return await _context.GroupUsers
                .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);
        }

        public async Task<IEnumerable<GroupUser>> GetGroupsbyUserIdAsync(int userId)
        {
            return await _context.GroupUsers
                .Where(gu => gu.UserId == userId)
                .Include(gu => gu.Group)
                .ToListAsync();
        }
    }
}