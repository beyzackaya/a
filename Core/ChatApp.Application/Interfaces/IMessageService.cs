using ChatApp.Application.DTOs.MessageDTOs;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Message?> GetByIdAsync(int id);
        Task<IEnumerable<MessageDto>> GetMessagesByGroupIdAsync(int groupId, int currentUserId);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId);
        Task<Message> CreateMessageAsync(int senderId, int groupId, string content);
        Task UpdateMessageAsync(int messageId, string newContent);
        Task DeleteMessageAsync(int id);
    }
}