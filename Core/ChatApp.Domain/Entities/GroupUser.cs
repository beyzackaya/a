using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities
{
    public class GroupUser

    {
        public int GroupUserId { get; set; }
        public Group Group { get; set; } = null!;
        public int GroupId { get; set; }
        public User User { get; set; } = null!;
        public int UserId { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    
}