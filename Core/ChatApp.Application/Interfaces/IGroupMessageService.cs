using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.MessageDTOs;

namespace ChatApp.Application.Interfaces
{
    public interface IGroupMessageService
    {
        Task<List<MessageDto>> GetGroupMessagesAsync(int groupId, int userId);
        Task SendMessageAsync(int groupId, int userId, string content);
    }
}