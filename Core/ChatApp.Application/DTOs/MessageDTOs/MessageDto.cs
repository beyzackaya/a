namespace ChatApp.Application.DTOs.MessageDTOs
{
    public class MessageDto
    {        public int MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string SenderName { get; set; } = null!;
        public string userName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public bool IsMine { get; set; }
        public int SenderId { get; set; }
    }
}
