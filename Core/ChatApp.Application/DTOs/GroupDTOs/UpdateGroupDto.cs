using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.GroupDTOs
{
    public class UpdateGroupDto
    {
        public int GroupId { get; set; }
        public string? Name { get; set; } 
    }
}