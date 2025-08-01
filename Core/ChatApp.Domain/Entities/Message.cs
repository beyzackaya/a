using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace ChatApp.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public GroupUser? GroupUser { get; set; } = null!;
        public int GroupUserId { get; set; }
        public int GroupId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;


        [Required]
        public string Content { get; set; } = string.Empty;
        public bool isDeleted { get; set; } = false;
        public bool isEdited { get; set; } = false;
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;
        public Group? Group { get; set; }   
        public ICollection<MessageReadStatus> MessageReadStatuses { get; set; } = new List<MessageReadStatus>();


       
    }
}