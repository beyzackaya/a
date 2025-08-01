using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Application.DTOs.AuthenticationDTOs;

namespace ChatApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto loginDto);
        // Task<bool> UserExistsAsync(string username, string email);
        Task<bool> RegisterAsync(RegisterDto registerDto);

           
    }
}