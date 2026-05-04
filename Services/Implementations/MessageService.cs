using ChatAppMVC.DTOs;
using ChatAppMVC.Helpers;
using ChatAppMVC.Models;
using ChatAppMVC.Repository.Interfaces;
using ChatAppMVC.Services.Interfaces;

namespace ChatAppMVC.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<ApiResponse<MessageResponseDto>> SendMessageAsync(int senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content
            };

            await _messageRepository.AddMessageAsync(message);
            await _messageRepository.SaveChangesAsync();

            var response = new MessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = message.SentAt
            };

            return new ApiResponse<MessageResponseDto>(
                true,
                "Message sent",
                response,
                200
            );
        }

        public async Task<ApiResponse<List<MessageResponseDto>>> GetMessagesAsync(int userId1, int userId2)
        {
            var messages = await _messageRepository.GetMessagesAsync(userId1, userId2);

            var response = messages.Select(m => new MessageResponseDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.Username, 
                ReceiverId = m.ReceiverId,
                Content = m.IsDeleted ? "This message was deleted" : m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead,
                IsDeleted = m.IsDeleted
            }).ToList();

            return new ApiResponse<List<MessageResponseDto>>(
                true,
                "Messages fetched",
                response,
                200
            );
        }

        public async Task<ApiResponse<string>> MarkMessagesAsReadAsync(int currentUserId, int senderId)
        {
            var messages = await _messageRepository.GetMessagesAsync(currentUserId, senderId);
            var unreadMessages = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead).ToList();

            foreach (var m in unreadMessages)
            {
                m.IsRead = true;
            }

            if (unreadMessages.Any())
            {
                await _messageRepository.SaveChangesAsync();
            }

            return new ApiResponse<string>(true, "Messages marked as read", null, 200);
        }

        public async Task<ApiResponse<string>> DeleteMessageAsync(int currentUserId, int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null) return new ApiResponse<string>(false, "Message not found", null, 404);
            if (message.SenderId != currentUserId) return new ApiResponse<string>(false, "Unauthorized", null, 403);

            message.IsDeleted = true;
            await _messageRepository.SaveChangesAsync();

            return new ApiResponse<string>(true, "Message deleted", null, 200);
        }
    }
}