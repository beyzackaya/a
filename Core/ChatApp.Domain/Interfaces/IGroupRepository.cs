using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task<Group?> GetByIdAsync(int id);
        Task<IEnumerable<Group>> GetAllAsync();
        Task AddAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
        // Task<Group?> GetByNameAsync(string groupName);
        Task<IEnumerable<Group>> GetGroupsByNameAsync(string name);







        Task<List<Group>> GetUserGroupsWithLastMessagesAsync(int userId);
        Task<List<Message>> GetGroupMessagesAsync(int groupId);

    }
}