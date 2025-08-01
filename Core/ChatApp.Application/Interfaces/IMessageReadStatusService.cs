using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageReadStatusService
    {
        Task<IEnumerable<MessageReadStatus>> GetByMessageIdAsync(int messageId);
        Task SetReadStatusAsync(int messageId, bool isRead);
    }
}