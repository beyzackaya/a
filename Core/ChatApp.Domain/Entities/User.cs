using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [MaxLength(20)]
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserOnlineStatus> UserOnlineStatuses { get; set; } = new List<UserOnlineStatus>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

 
    }

    
}