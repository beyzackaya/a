using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces
{
    public interface IGroupUserRepository
    {
        Task<GroupUser?> GetByIdAsync(int id);
        Task<IEnumerable<GroupUser>> GetGroupsbyUserIdAsync(int userId);
        Task<IEnumerable<GroupUser>> GetUsersByGroupIdAsync(int groupId);
        Task AddAsync(GroupUser groupUser);
        Task DeleteAsync(int id);
        Task<GroupUser?> GetByGroupIdAndUserIdAsync(int groupId, int userId);
    }
        
}