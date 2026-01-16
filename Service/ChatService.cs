using Service.Models;
using Service.Interface;
using Service.RepoInterface;

namespace Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepo _chatRepo;
        private readonly IUserRepo _userRepo;
        private readonly ICurrentUserService _currentUser;

        public ChatService(IChatRepo chatRepo, IUserRepo userRepo, ICurrentUserService currentUser)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _currentUser = currentUser;
        }

        public async Task<List<MessageModel>> GetConversationAsync(Guid targetUserId)
        {
            var myId = _currentUser.UserGuid;

            var messages = await _chatRepo.GetMessagesAsync(myId, targetUserId);

            foreach (var msg in messages)
            {
                msg.IsFromMe = (msg.SenderID == myId);
            }

            return messages;
        }

        public async Task SendMessageAsync(Guid receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            var myId = _currentUser.UserGuid;

            bool isBlocked = await _chatRepo.HasBlockRelationshipAsync(receiverId, myId);
            if (isBlocked)
            {
                throw new UnauthorizedAccessException("You cannot send messages to this user.");
            }

            var newMessage = new MessageModel
            {
                SenderID = myId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            await _chatRepo.SendMessageAsync(receiverId, newMessage);
        }

        public async Task<List<ContactModel>> GetMyContactsAsync()
        {
            return await _chatRepo.GetContactsByOwnerAsync(_currentUser.UserGuid);
        }

        public async Task<UserModel?> SearchUserByExactNameAsync(string username)
        {
            return await _userRepo.GetByUsernameAsync(username);
        }

        public async Task AddContactAsync(string username)
        {
            var targetUser = await _userRepo.GetByUsernameAsync(username);
            if (targetUser == null) throw new KeyNotFoundException("User not found.");

            if (targetUser.UserID == _currentUser.UserGuid)
                throw new InvalidOperationException("You cannot add yourself.");

            await _chatRepo.AddContactAsync(_currentUser.UserGuid, targetUser.UserID);
        }

        public async Task MarkConversationAsReadAsync(Guid targetUserId)
        {
            await _chatRepo.MarkConversationAsReadAsync(_currentUser.UserGuid, targetUserId);
        }

        public async Task BlockUserAsync(Guid userId)
        {
            bool alreadyBlocked = await _chatRepo.IsBlockedByMeAsync(_currentUser.UserGuid, userId);
            if (!alreadyBlocked)
            {
                await _chatRepo.CreateBlockAsync(_currentUser.UserGuid, userId);
            }
        }

        public async Task UnblockUserAsync(Guid userId)
        {
            await _chatRepo.DeleteBlockAsync(_currentUser.UserGuid, userId);
        }

        public async Task<bool> IsUserBlockedByMeAsync(Guid userId)
        {
            return await _chatRepo.IsBlockedByMeAsync(_currentUser.UserGuid, userId);
        }

        public async Task<bool> IsUserBlockedAsync(Guid userId)
        {
            return await _chatRepo.HasBlockRelationshipAsync(_currentUser.UserGuid, userId);
        }
    }
}