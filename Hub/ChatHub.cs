using Microsoft.AspNetCore.SignalR;

namespace ChatAppMVC.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            await Clients.User(receiverId.ToString())
                .SendAsync("ReceiveMessage", senderId, message);
        }

        public async Task NotifyTyping(int senderId, int receiverId, bool isTyping)
        {
            await Clients.User(receiverId.ToString())
                .SendAsync("UserTyping", senderId, isTyping);
        }

        public async Task MarkAsRead(int senderId, int receiverId)
        {
            await Clients.User(receiverId.ToString())
                .SendAsync("MessageRead", senderId);
        }
    }
}