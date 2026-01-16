using Service.Models;

namespace Service.RepoInterface
{
    public interface IChatRepo
    {
        // Messages
        Task<List<MessageModel>> GetMessagesAsync(Guid user1, Guid user2);
        Task SendMessageAsync(Guid receiverId, MessageModel message);

        // Contacts
        Task<List<ContactModel>> GetContactsByOwnerAsync(Guid ownerId);
        Task AddContactAsync(Guid ownerId, Guid targetId);

        // Blocks
        Task CreateBlockAsync(Guid blockerId, Guid blockedId);
        Task DeleteBlockAsync(Guid blockerId, Guid blockedId);
        Task<bool> IsBlockedByMeAsync(Guid blockerId, Guid blockedId);
        Task<bool> HasBlockRelationshipAsync(Guid userA, Guid userB);
        Task MarkConversationAsReadAsync(Guid ownerId, Guid contactId);
    }
}