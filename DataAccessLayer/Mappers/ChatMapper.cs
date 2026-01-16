using DataAccessLayer.DataTransferObjects;
using Service.Models;

namespace DataAccessLayer.Mappers
{
    public static class ChatMapper
    {
        public static MessageModel ToModel(ChatMessageDTO dto, Guid currentUserId)
        {
            return new MessageModel
            {
                MessageID = dto.MessageID,
                Content = dto.MessageContent,
                SenderID = dto.SenderID,
                SentAt = dto.SentAt,
                IsFromMe = dto.SenderID == currentUserId,
                IsRead = dto.IsRead
            };
        }

        public static ChatMessageDTO ToDTO(MessageModel model, Guid receiverId)
        {
            return new ChatMessageDTO
            {
                MessageID = model.MessageID,
                SenderID = model.SenderID,
                ReceiverID = receiverId,
                MessageContent = model.Content,
                SentAt = model.SentAt,
                IsRead = model.IsRead
            };
        }

        public static List<MessageModel> ToModelList(IEnumerable<ChatMessageDTO> dtos, Guid currentUserId)
        {
            return dtos.Select(d => ToModel(d, currentUserId)).ToList();
        }
    }
}