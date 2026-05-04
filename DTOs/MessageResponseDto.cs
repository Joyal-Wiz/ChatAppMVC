public class MessageResponseDto
{
    public int SenderId { get; set; }
    public string SenderName { get; set; } 
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
}