namespace OkaneFlow.ViewModels;


public class ChatMessageVM
{
    public string Content { get; set; } = string.Empty;
    public string TimeLabel { get; set; } = string.Empty;
    public string AlignmentClass => IsFromMe ? "flex-row-reverse" : "";
    public string BubbleColor => IsFromMe ? "bg-primary text-white" : "bg-light border text-dark";
    public bool IsFromMe { get; set; }
}