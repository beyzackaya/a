using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.GroupUserDTOs;
using ChatApp.Application.DTOs.MessageDTOs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Application.Services
{
public class GroupMessageService : IGroupMessageService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMessageRepository _messageRepository;

    public GroupMessageService(IGroupRepository groupRepository, IMessageRepository messageRepository)
    {
        _groupRepository = groupRepository;
        _messageRepository = messageRepository;
    }

    public async Task<List<MessageDto>> GetGroupMessagesAsync(int groupId, int userId)
    {
        var messages = await _groupRepository.GetGroupMessagesAsync(groupId);

        return messages.Select(m => new MessageDto
        {
            MessageId = m.MessageId,
            Content = m.Content,
            SenderName = m.Sender.Name,
            SentAt = m.SentAt,
            IsMine = m.SenderId == userId

        }).ToList();
    }

    public async Task SendMessageAsync(int groupId, int userId, string content)
    {
        var message = new Message
        {
            SenderId = userId,
            GroupId = groupId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message);
    }
}
}