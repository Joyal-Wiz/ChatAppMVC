using Microsoft.AspNetCore.SignalR;

namespace ChatAppMVC.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            // Send message to receiver
            await Clients.User(receiverId.ToString())
                .SendAsync("ReceiveMessage", senderId, message);
        }
    }
}