using ChatApp.Application.DTOs.MessageDTOs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupUserRepository _groupUserRepository;

        public MessageService(IMessageRepository messageRepository, IGroupUserRepository groupUserRepository)
        {
            _messageRepository = messageRepository;
            _groupUserRepository = groupUserRepository;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _messageRepository.GetByIdAsync(id);
        }
        

        public async Task<IEnumerable<MessageDto>> GetMessagesByGroupIdAsync(int groupId, int currentUserId)
        {
            try
            {
                var messages = await _messageRepository.GetMessagesByGroupIdAsync(groupId);

                return messages.Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.UserName ?? "Bilinmeyen",
                    IsEdited = m.isEdited,
                    IsMine = m.SenderId == currentUserId
                }).OrderBy(m => m.SentAt);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Mesajlar yüklenirken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(int userId)
        {
            return await _messageRepository.GetMessagesByUserIdAsync(userId);
        }

        public async Task<Message> CreateMessageAsync(int senderId, int groupId, string content)
        {
            try
            {
                // Kullanıcının gruba üye olup olmadığını kontrol et
                var groupUser = await _groupUserRepository.GetByGroupIdAndUserIdAsync(groupId, senderId);
                if (groupUser == null)
                {
                    throw new UnauthorizedAccessException("Bu gruba mesaj gönderme yetkiniz yok.");
                }

                var message = new Message
                {
                    SenderId = senderId,
                    GroupUserId = groupUser.GroupUserId,
                    GroupId = groupId,
                    Content = content.Trim(),
                    isDeleted = false,
                    isEdited = false,
                    SentAt = DateTime.UtcNow
                };

                await _messageRepository.AddAsync(message);
                return message;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Mesaj gönderilirken bir hata oluştu.", ex);
            }
        }

        public async Task UpdateMessageAsync(int messageId, string newContent)
        {
            try
            {
                var msg = await _messageRepository.GetByIdAsync(messageId);
                if (msg == null) 
                {
                    throw new ArgumentException("Mesaj bulunamadı.");
                }

                msg.Content = newContent.Trim();
                msg.isEdited = true;
                await _messageRepository.UpdateAsync(msg);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Mesaj güncellenirken bir hata oluştu.", ex);
            }
        }

        public async Task DeleteMessageAsync(int id)
        {
            try
            {
                await _messageRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Mesaj silinirken bir hata oluştu.", ex);
            }
        } 
    }
}