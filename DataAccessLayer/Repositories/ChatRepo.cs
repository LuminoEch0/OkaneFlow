using DataAccessLayer.DataTransferObjects;
using Service.RepoInterface;
using Microsoft.Data.SqlClient;
using System.Data;
using Service.Models;
using DataAccessLayer.Mappers;

namespace DataAccessLayer.Repositories
{
    public class ChatRepo : IChatRepo
    {
        private readonly ConnectionManager _dbManager;

        public ChatRepo(ConnectionManager dbManager) => _dbManager = dbManager;

        public async Task<List<MessageModel>> GetMessagesAsync(Guid user1, Guid user2)
        {
            // Table: ChatMessage, Column: MessageContent
            string sql = @"SELECT MessageID, SenderID, ReceiverID, MessageContent, SentAt, IsRead 
                           FROM [ChatMessages] 
                           WHERE (SenderID = @u1 AND ReceiverID = @u2) 
                           OR (SenderID = @u2 AND ReceiverID = @u1) 
                           ORDER BY SentAt ASC";

            var messages = new List<MessageModel>();

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u1", user1);
                cmd.Parameters.AddWithValue("@u2", user2);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dto = new ChatMessageDTO
                        {
                            MessageID = reader.GetGuid(0),
                            SenderID = reader.GetGuid(1),
                            ReceiverID = reader.GetGuid(2),
                            MessageContent = reader.GetString(3),
                            SentAt = reader.GetDateTime(4),
                            IsRead = reader.GetBoolean(5)
                        };
                        messages.Add(ChatMapper.ToModel(dto, user1));
                    }
                }
            }
            return messages;
        }

        public async Task SendMessageAsync(Guid receiverId, MessageModel message)
        {
            // Table: ChatMessage, Column: MessageContent
            string sql = "INSERT INTO [ChatMessages] (MessageID, SenderID, ReceiverID, MessageContent, SentAt, IsRead) VALUES (@id, @s, @r, @c, @t, 0)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@s", message.SenderID);
                cmd.Parameters.AddWithValue("@r", receiverId);
                cmd.Parameters.AddWithValue("@c", message.Content);
                cmd.Parameters.AddWithValue("@t", DateTime.UtcNow);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<ContactModel>> GetContactsByOwnerAsync(Guid ownerId)
        {
            // Table: UserContact, Columns: OwnerID, ContactID
            string sql = @"SELECT DISTINCT u.UserID, u.Username, c.AddedAt 
                           FROM [User] u
                           INNER JOIN [UserContact] c ON (u.UserID = c.ContactID AND c.OwnerID = @ownerId)
                                                      OR (u.UserID = c.OwnerID AND c.ContactID = @ownerId)
                           WHERE u.UserID <> @ownerId
                           ORDER BY u.Username ASC";

            var contacts = new List<ContactModel>();

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ownerId", ownerId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        contacts.Add(new ContactModel
                        {
                            TargetUserID = reader.GetGuid(0),
                            Username = reader.GetString(1),
                            AddedAt = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2)
                        });
                    }
                }
            }
            return contacts;
        }

        public async Task AddContactAsync(Guid ownerId, Guid targetId)
        {
            // Table: UserContact, Columns: UserContactID, OwnerID, ContactID, AddedAt
            string sql = "INSERT INTO [UserContact] (UserContactID, OwnerID, ContactID, AddedAt) VALUES (@id, @o, @t, @ts)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@o", ownerId);
                cmd.Parameters.AddWithValue("@t", targetId);
                cmd.Parameters.AddWithValue("@ts", DateTime.UtcNow);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task MarkConversationAsReadAsync(Guid ownerId, Guid contactId)
        {
            string sql = "UPDATE [ChatMessages] SET IsRead = 1 WHERE SenderID = @contactId AND ReceiverID = @ownerId AND IsRead = 0";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                cmd.Parameters.AddWithValue("@contactId", contactId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateBlockAsync(Guid blockerId, Guid blockedId)
        {
            // Table: BlockedUsers (plural)
            string sql = "INSERT INTO [BlockedUsers] (BlockedUserID, BlockerID, BlockedID, BlockedAt) VALUES (@id, @br, @bd, @at)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@br", blockerId);
                cmd.Parameters.AddWithValue("@bd", blockedId);
                cmd.Parameters.AddWithValue("@at", DateTime.UtcNow);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteBlockAsync(Guid blockerId, Guid blockedId)
        {
            string sql = "DELETE FROM [BlockedUsers] WHERE BlockerID = @br AND BlockedID = @bd";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@br", blockerId);
                cmd.Parameters.AddWithValue("@bd", blockedId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> IsBlockedByMeAsync(Guid blockerId, Guid blockedId)
        {
            string sql = "SELECT COUNT(1) FROM [BlockedUsers] WHERE BlockerID = @br AND BlockedID = @bd";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@br", blockerId);
                cmd.Parameters.AddWithValue("@bd", blockedId);

                var result = await cmd.ExecuteScalarAsync();
                return result != null && (int)result > 0;
            }
        }

        public async Task<bool> HasBlockRelationshipAsync(Guid userA, Guid userB)
        {
            // Table: BlockedUsers (plural)
            string sql = @"SELECT COUNT(1) FROM [BlockedUsers] 
                           WHERE (BlockerID = @u1 AND BlockedID = @u2) 
                           OR (BlockerID = @u2 AND BlockedID = @u1)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u1", userA);
                cmd.Parameters.AddWithValue("@u2", userB);

                var result = await cmd.ExecuteScalarAsync();
                return result != null && (int)result > 0;
            }
        }
    }
}