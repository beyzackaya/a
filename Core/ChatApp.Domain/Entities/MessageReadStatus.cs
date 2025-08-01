using System;

namespace ChatApp.Domain.Entities
{
    public class MessageReadStatus
    {
        public int MessageId { get; set; }
        public Boolean IsRead { get; set; } = false;
        public Message Message { get; set; } = null!;
        public int MessageReadStatusId { get; set; }
    }
}