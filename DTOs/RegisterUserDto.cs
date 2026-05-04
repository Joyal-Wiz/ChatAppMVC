using Microsoft.AspNetCore.Http;

namespace ChatAppMVC.DTOs
{
    public class RegisterUserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}