using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IGroupUserService
    {
        Task<GroupUser?> GetByIdAsync(int id);
        Task<IEnumerable<GroupUser>> GetUsersByGroupIdAsync(int groupId);
        Task AddUserToGroupAsync(int userId, int groupId);
        Task RemoveGroupUserAsync(int groupUserId);
        Task<IEnumerable<GroupUser>> GetGroupsbyUserIdAsync(int userId);
        Task RemoveUserFromGroupAsync(int userId, int groupId);
    }
}