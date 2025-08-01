using ChatApp.Application.DTOs.GroupDTOs;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IGroupService
    {
        Task<Group?> GetGroupByIdAsync(int id);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task CreateGroupAsync(string groupName);
        Task<Group> CreateGroupWithUsersAsync(string groupName, List<int> userIds, int creatorUserId);
        Task<Group?> FindExistingGroupAsync(List<int> userIds);
        // Task DeleteGroupAsync(int id);
        // Task<Group?> GetGroupByNameAsync(string name);
        Task UpdateGroupAsync(UpdateGroupDto updateGroupDto);
        Task<IEnumerable<GroupUser>> GetGroupUsersAsync(int groupId);
    }
}