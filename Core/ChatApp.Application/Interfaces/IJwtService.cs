using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        // bool ValidateToken(string token);
        // int GetUserIdFromToken(string token);
    }
}