using Microsoft.AspNetCore.SignalR;
using ChatAppMVC.Services;
using System.Security.Claims;

namespace ChatAppMVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IOnlineUserTracker _tracker;

        public ChatHub(IOnlineUserTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                _tracker.AddUser(userId, Context.ConnectionId);
                await Clients.Others.SendAsync("UserStatusChanged", userId, true);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                _tracker.RemoveUser(userId, Context.ConnectionId);
                if (!_tracker.IsUserOnline(userId))
                {
                    await Clients.Others.SendAsync("UserStatusChanged", userId, false);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int senderId, int receiverId, string message, int messageId)
        {
            await Clients.User(receiverId.ToString())
                .SendAsync("ReceiveMessage", senderId, message, messageId);
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

        public async Task DeleteMessage(int senderId, int receiverId, int messageId)
        {
            await Clients.User(receiverId.ToString())
                .SendAsync("MessageDeleted", messageId);
        }
    }
}