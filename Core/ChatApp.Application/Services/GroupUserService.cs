using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;
using System.Linq;

namespace ChatApp.Application.Services
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IGroupUserRepository _groupUserRepository;

        public GroupUserService(IGroupUserRepository groupUserRepository)
        {
            _groupUserRepository = groupUserRepository;
        }

        public async Task<GroupUser?> GetByIdAsync(int id)
        {
            return await _groupUserRepository.GetByIdAsync(id);
        }
        
        public async Task<IEnumerable<GroupUser>> GetGroupsbyUserIdAsync(int userId)
        {
            return await _groupUserRepository.GetGroupsbyUserIdAsync(userId);
        }   

        public async Task<IEnumerable<GroupUser>> GetUsersByGroupIdAsync(int groupId)
        {
            return await _groupUserRepository.GetUsersByGroupIdAsync(groupId);
        }

        //olan gruba yeni kullanıcı ekleme

        public async Task AddUserToGroupAsync(int userId, int groupId)
        {
            var groupUser = new GroupUser
            {
                UserId = userId,
                GroupId = groupId
            };
            await _groupUserRepository.AddAsync(groupUser);
        }

        public async Task RemoveGroupUserAsync(int groupUserId)
        {
            await _groupUserRepository.DeleteAsync(groupUserId);
        }

        public async Task RemoveUserFromGroupAsync(int userId, int groupId)
        {
            var groupUsers = await _groupUserRepository.GetUsersByGroupIdAsync(groupId);
            var groupUser = groupUsers.FirstOrDefault(gu => gu.UserId == userId);
            
            if (groupUser != null)
            {
                await _groupUserRepository.DeleteAsync(groupUser.GroupUserId);
            }
        }
    }
}