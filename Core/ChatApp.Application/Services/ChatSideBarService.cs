using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.Interfaces;
using ChatApp.Application.DTOs.GroupUserDTOs;
using ChatApp.Application.DTOs.UserDTOs;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class ChatSideBarService : IChatSideBarService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;

        public ChatSideBarService(IGroupRepository groupRepository, IGroupUserRepository groupUserRepository)
        {
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
        }

        public async Task<List<GroupChatListItemDto>> GetUserGroupChatsAsync(int userId)
        {
            var groups = await _groupRepository.GetUserGroupsWithLastMessagesAsync(userId);

            var result = groups.Select(g => new GroupChatListItemDto
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName,
                LastMessageContent = g.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content,
                LastMessageTime = g.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt
            })
            .OrderByDescending(g => g.LastMessageTime ?? DateTime.MinValue)
            .ToList();

            return result;
        }

        public async Task<List<GroupChatListItemDto>> GetUserGroupChatsWithParticipantsAsync(int userId)
        {
            var groups = await _groupRepository.GetUserGroupsWithLastMessagesAsync(userId);
            var result = new List<GroupChatListItemDto>();

            foreach (var group in groups)
            {
                var groupUsers = await _groupUserRepository.GetUsersByGroupIdAsync(group.GroupId);
                var participants = groupUsers.Select(gu => new UserDto
                {
                    UserId = gu.User.UserId,
                    Name = gu.User.Name,
                    LastName = gu.User.LastName,
                    UserName = gu.User.UserName,
                    Email = gu.User.Email,
                    PhoneNumber = gu.User.PhoneNumber
                }).ToList();

                string displayName = group.GroupName;
                if (group.IsPrivateChat && participants.Count == 2)
                {
                    var otherUser = participants.FirstOrDefault(p => p.UserId != userId);
                    if (otherUser != null)
                    {
                        displayName = $"{otherUser.Name} {otherUser.LastName}";
                    }
                }

                var lastMessage = group.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                result.Add(new GroupChatListItemDto
                {
                    GroupId = group.GroupId,
                    GroupName = displayName,
                    LastMessageContent = lastMessage?.Content,
                    LastMessageTime = lastMessage?.SentAt,
                    Participants = participants,
                    IsPrivateChat = group.IsPrivateChat
                });
            }

            return result
                .OrderByDescending(r => r.LastMessageTime ?? DateTime.MinValue)
                .ToList();
        }
    }
}