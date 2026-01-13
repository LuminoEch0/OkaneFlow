namespace Service.Models
{
    public class ChatMessageModel
    {
        public Guid MessageID { get; set; }
        public Guid SenderID { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsFromMe { get; set; } // Business logic helper
    }
}