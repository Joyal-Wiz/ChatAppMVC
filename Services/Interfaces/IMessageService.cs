using ChatAppMVC.DTOs;
using ChatAppMVC.Helpers;

namespace ChatAppMVC.Services.Interfaces
{
    public interface IMessageService
    {
        Task<ApiResponse<string>> SendMessageAsync(int senderId, SendMessageDto dto);
        Task<ApiResponse<List<MessageResponseDto>>> GetMessagesAsync(int userId1, int userId2);
        Task<ApiResponse<string>> MarkMessagesAsReadAsync(int currentUserId, int senderId);
    }
}