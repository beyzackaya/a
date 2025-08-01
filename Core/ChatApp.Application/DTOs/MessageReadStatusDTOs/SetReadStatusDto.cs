using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.MessageReadStatusDTOs
{
    public class SetReadStatusDto
    {
        public int MessageId { get; set; }
        public bool IsRead { get; set; }
    }
}