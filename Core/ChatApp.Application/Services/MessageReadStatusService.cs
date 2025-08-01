using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class MessageReadStatusService : IMessageReadStatusService
    {
        private readonly IMessageReadStatusRepository _repository;

        public MessageReadStatusService(IMessageReadStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MessageReadStatus>> GetByMessageIdAsync(int messageId)
        {
            return await _repository.GetByMessageIdAsync(messageId);
        }

        public async Task SetReadStatusAsync(int messageId, bool isRead)
        {
            var statuses = await _repository.GetByMessageIdAsync(messageId);
            foreach (var status in statuses)
            {
                status.IsRead = isRead;
                await _repository.UpdateAsync(status);
            }
        }
    }
}