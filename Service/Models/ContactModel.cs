namespace Service.Models;

public class ContactModel
{
    public Guid ContactID { get; set; }
    public Guid TargetUserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastInteraction { get; set; }
}