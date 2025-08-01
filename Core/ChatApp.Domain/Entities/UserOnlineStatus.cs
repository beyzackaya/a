using System;

namespace ChatApp.Domain.Entities
{
    public class UserOnlineStatus
    {
        public int UserId { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
        public int UserOnlineStatusId { get; set; }

        public User User { get; set; } = null!;
        
    }
}