using ChatApp.Application.DTOs.GroupDTOs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;

        public GroupService(IGroupRepository groupRepository, IGroupUserRepository groupUserRepository)
        {
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
        }

        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _groupRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _groupRepository.GetAllAsync();
        }

        public async Task CreateGroupAsync(string groupName)
        {
            var group = new Group { GroupName = groupName };
            await _groupRepository.AddAsync(group);
        }

        // public async Task DeleteGroupAsync(int id)
        // {
        //     await _groupRepository.DeleteAsync(id);
        // }
        // public async Task DeleteGroupAsync(string groupName)
        // {
        //     var group = await _groupRepository.GetByNameAsync(groupName);
        //     if (group != null)
        //     {
        //         await _groupRepository.DeleteAsync(group.GroupId);
        //     }
        // }
        // public async Task<Group?> GetGroupByNameAsync(string name)
        // {
        //     return await _groupRepository.GetByNameAsync(name);
        // }
        public async Task UpdateGroupAsync(UpdateGroupDto updateGroupDto)
        {
            if (updateGroupDto == null)
            {
                throw new ArgumentNullException(nameof(updateGroupDto));
            }

            if (string.IsNullOrWhiteSpace(updateGroupDto.Name))
            {
                throw new ArgumentException("Group name cannot be null or empty.", nameof(updateGroupDto.Name));
            }

            var group = await _groupRepository.GetByIdAsync(updateGroupDto.GroupId);
            if (group != null)
            {
                group.GroupName = updateGroupDto.Name;
                await _groupRepository.UpdateAsync(group);
            }
            else
            {
                throw new KeyNotFoundException($"Group with ID {updateGroupDto.GroupId} not found.");
            }
        }

        public async Task<Group> CreateGroupWithUsersAsync(string groupName, List<int> userIds, int creatorUserId)
        {
            try
            {
                var allUserIds = new List<int>(userIds);
                if (!allUserIds.Contains(creatorUserId))
                {
                    allUserIds.Add(creatorUserId);
                }

                var existingGroup = await FindExistingGroupAsync(allUserIds);
                if (existingGroup != null)
                {
                    return existingGroup;
                }

                var group = new Group 
                { 
                    GroupName = groupName,
                    CreatedByUserId = creatorUserId,  
                    CreatedAt = DateTime.UtcNow,
                    IsPrivateChat = allUserIds.Count == 2 
                };
                await _groupRepository.AddAsync(group);

                foreach (var userId in allUserIds)
                {
                    var groupUser = new GroupUser
                    {
                        GroupId = group.GroupId,
                        UserId = userId,
                        JoinedAt = DateTime.UtcNow,
                        IsAdmin = (userId == creatorUserId)
                    };
                    await _groupUserRepository.AddAsync(groupUser);
                }

                return group;
            }
            catch (Exception ex)
            {
                throw new Exception("Grup oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<Group?> FindExistingGroupAsync(List<int> userIds)
        {
            try
            {
                // Kullanıcı sayısı aynı olan grupları bul
                var groups = await _groupRepository.GetAllAsync();
                
                foreach (var group in groups)
                {
                    var groupUsers = await _groupUserRepository.GetUsersByGroupIdAsync(group.GroupId);
                    var groupUserIds = groupUsers.Select(gu => gu.UserId).OrderBy(id => id).ToList();
                    var sortedUserIds = userIds.OrderBy(id => id).ToList();
                    
                    // Eğer user ID'leri tam olarak eşleşiyorsa, grup zaten mevcut
                    if (groupUserIds.SequenceEqual(sortedUserIds))
                    {
                        return group;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Mevcut grup aranırken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<GroupUser>> GetGroupUsersAsync(int groupId)
        {
            try
            {
                return await _groupUserRepository.GetUsersByGroupIdAsync(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception("Grup üyeleri alınırken bir hata oluştu.", ex);
            }
        }
    }
}