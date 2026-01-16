using Service.Models;

namespace Service.Interface;

public interface IChatService
{
    // Messaging
    Task<List<MessageModel>> GetConversationAsync(Guid targetUserId);
    Task SendMessageAsync(Guid receiverId, string content);

    // Context / Sidebar
    Task<List<ContactModel>> GetMyContactsAsync();

    // Privacy & Search
    Task<UserModel?> SearchUserByExactNameAsync(string username);
    Task AddContactAsync(string username);
    Task MarkConversationAsReadAsync(Guid targetUserId);
    Task BlockUserAsync(Guid userId);
    Task UnblockUserAsync(Guid userId);
    Task<bool> IsUserBlockedByMeAsync(Guid userId);
    Task<bool> IsUserBlockedAsync(Guid userId);
}