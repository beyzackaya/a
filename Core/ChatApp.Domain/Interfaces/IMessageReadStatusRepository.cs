using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces
{
    public interface IMessageReadStatusRepository
    {
        Task<MessageReadStatus?> GetByIdAsync(int id);
        Task<IEnumerable<MessageReadStatus>> GetByMessageIdAsync(int messageId);
        Task AddAsync(MessageReadStatus status);
        Task UpdateAsync(MessageReadStatus status);
    }
}