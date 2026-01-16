using OkaneFlow.ViewModels;
using Service.Models;

namespace OkaneFlow.Mappers;

public static class ChatUIMapper
{
    public static ChatMessageVM ToVM(MessageModel model) => new()
    {
        Content = model.Content,
        TimeLabel = model.SentAt.ToString("HH:mm"),
        IsFromMe = model.IsFromMe,
        IsRead = model.IsRead
    };
}