using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.UserIOnlineStatus
{
    public class UserStatusDto
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; } = string.Empty;
    }
}