using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.GroupUserDTOs;

namespace ChatApp.Application.Interfaces
{
    public interface IChatSideBarService
    {
        Task<List<GroupChatListItemDto>> GetUserGroupChatsAsync(int userId);
        Task<List<GroupChatListItemDto>> GetUserGroupChatsWithParticipantsAsync(int userId);
    }
}