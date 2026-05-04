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

        public async Task<ApiResponse<string>> SendMessageAsync(int senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content
            };

            await _messageRepository.AddMessageAsync(message);
            await _messageRepository.SaveChangesAsync();

            return new ApiResponse<string>(
                true,
                "Message sent",
                null,
                200
            );
        }

        public async Task<ApiResponse<List<MessageResponseDto>>> GetMessagesAsync(int userId1, int userId2)
        {
            var messages = await _messageRepository.GetMessagesAsync(userId1, userId2);

            var response = messages.Select(m => new MessageResponseDto
            {
                SenderId = m.SenderId,
                SenderName = m.Sender.Username, // ✅ IMPORTANT
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                SentAt = m.SentAt
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
    }
}