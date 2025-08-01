using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.GroupDTOs
{
    public class CreateGroupDto
    {
        public string GroupName { get; set; } = null!;
        public List<int> UserIds { get; set; } = new();

    }
}