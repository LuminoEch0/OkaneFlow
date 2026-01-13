namespace Service.Models;

public class MessageModel
{
    public Guid MessageID { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsFromMe { get; set; } // Business logic flag
    public Guid SenderID { get; set; }
}