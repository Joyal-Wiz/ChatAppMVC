using System.Collections.Concurrent;

namespace ChatAppMVC.Services
{
    public interface IOnlineUserTracker
    {
        void AddUser(int userId, string connectionId);
        void RemoveUser(int userId, string connectionId);
        bool IsUserOnline(int userId);
        IEnumerable<int> GetOnlineUsers();
    }

    public class OnlineUserTracker : IOnlineUserTracker
    {
        // Dictionary of UserId -> Set of ConnectionIds (to handle multiple tabs)
        private readonly ConcurrentDictionary<int, HashSet<string>> _onlineUsers = new();

        public void AddUser(int userId, string connectionId)
        {
            _onlineUsers.AddOrUpdate(userId, 
                _ => new HashSet<string> { connectionId },
                (_, connections) => {
                    lock (connections)
                    {
                        connections.Add(connectionId);
                    }
                    return connections;
                });
        }

        public void RemoveUser(int userId, string connectionId)
        {
            if (_onlineUsers.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _onlineUsers.TryRemove(userId, out _);
                    }
                }
            }
        }

        public bool IsUserOnline(int userId) => _onlineUsers.ContainsKey(userId);

        public IEnumerable<int> GetOnlineUsers() => _onlineUsers.Keys;
    }
}
