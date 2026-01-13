namespace DataAccessLayer.DataTransferObjects;

public class ChatMessageDTO
{
    public Guid MessageID { get; set; }
    public Guid SenderID { get; set; }
    public Guid ReceiverID { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}