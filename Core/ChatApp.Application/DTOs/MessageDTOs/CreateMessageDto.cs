using System.ComponentModel.DataAnnotations;

namespace ChatApp.Application.DTOs.MessageDTOs
{
    public class CreateMessageDto
    {
        [Required(ErrorMessage = "Mesaj içeriği gereklidir.")]
        [StringLength(1000, ErrorMessage = "Mesaj içeriği 1000 karakterden fazla olamaz.")]
        public string Content { get; set; } = string.Empty;
    }
}
