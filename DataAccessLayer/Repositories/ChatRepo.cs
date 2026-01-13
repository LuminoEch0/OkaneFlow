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
            // Table: ChatMessages, Column: MessageContent
            string sql = @"SELECT MessageID, SenderID, ReceiverID, MessageContent, SentAt 
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
                            SentAt = reader.GetDateTime(4)
                        };
                        messages.Add(ChatMapper.ToModel(dto, user1));
                    }
                }
            }
            return messages;
        }

        public async Task SendMessageAsync(Guid receiverId, MessageModel message)
        {
            // Table: ChatMessages, Column: MessageContent
            string sql = "INSERT INTO [ChatMessages] (SenderID, ReceiverID, MessageContent) VALUES (@s, @r, @c)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@s", message.SenderID);
                cmd.Parameters.AddWithValue("@r", receiverId);
                cmd.Parameters.AddWithValue("@c", message.Content);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<ContactModel>> GetContactsByOwnerAsync(Guid ownerId)
        {
            // Table: UserContact, Columns: OwnerID, ContactID
            string sql = @"SELECT c.OwnerID, c.ContactID, u.Username, c.AddedAt 
                           FROM [UserContact] c
                           INNER JOIN [User] u ON c.ContactID = u.UserID
                           WHERE c.OwnerID = @ownerId
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
                            // ContactID in your DB represents the Target/Friend UserID
                            TargetUserID = reader.GetGuid(1),
                            Username = reader.GetString(2),
                            AddedAt = reader.GetDateTime(3)
                        });
                    }
                }
            }
            return contacts;
        }

        public async Task AddContactAsync(Guid ownerId, Guid targetId)
        {
            // Table: UserContact, Columns: OwnerID, ContactID
            string sql = "INSERT INTO [UserContact] (OwnerID, ContactID) VALUES (@o, @t)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@o", ownerId);
                cmd.Parameters.AddWithValue("@t", targetId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateBlockAsync(Guid blockerId, Guid blockedId)
        {
            // Table: BlockedUsers (plural)
            string sql = "INSERT INTO [BlockedUsers] (BlockerID, BlockedID) VALUES (@br, @bd)";

            using (var conn = (SqlConnection)_dbManager.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@br", blockerId);
                cmd.Parameters.AddWithValue("@bd", blockedId);

                await cmd.ExecuteNonQueryAsync();
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