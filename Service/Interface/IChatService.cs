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
    Task BlockUserAsync(Guid userId);
    Task<bool> IsUserBlockedAsync(Guid userId);
}