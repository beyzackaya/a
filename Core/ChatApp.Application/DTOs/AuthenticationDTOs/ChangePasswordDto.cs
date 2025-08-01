using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.DTOs.AuthenticationDTOs
{
    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}