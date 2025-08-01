using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.UserDTOs
{
    public class UpdateUserByUsernameDto
    {
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        
    }
}