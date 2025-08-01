using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.GroupUserDTOs
{
    public class GroupUserDto
    {
        public int GroupUserId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
    }

}