using System;
using System.Collections.Generic;

namespace ChatApp.Domain.Entities
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public bool IsPrivateChat { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public int CreatedByUserId { get; set; }
    }
}