using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.MessageDTOs
{
    public class SendMessageDto
    {
        public int senderId { get; set; }
        public int groupUserId { get; set; }
        public string content { get; set; } = string.Empty;
    }
}