using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.AuthenticationDTOs
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
    
    }
}