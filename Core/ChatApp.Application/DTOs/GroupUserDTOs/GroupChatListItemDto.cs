using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.UserDTOs;

namespace ChatApp.Application.DTOs.GroupUserDTOs
{
    public class GroupChatListItemDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public List<UserDto> Participants { get; set; } = new List<UserDto>();
        public bool IsPrivateChat { get; set; } 
    }
}