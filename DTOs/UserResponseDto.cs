namespace ChatAppMVC.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsOnline { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public string? Status { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}