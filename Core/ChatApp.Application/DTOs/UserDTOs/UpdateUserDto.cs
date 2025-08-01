namespace ChatApp.Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}