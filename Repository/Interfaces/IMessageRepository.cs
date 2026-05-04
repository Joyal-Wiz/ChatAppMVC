using ChatAppMVC.Models;

namespace ChatAppMVC.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<List<Message>> GetMessagesAsync(int userId1, int userId2);
        Task<Message> GetLastMessageAsync(int userId1, int userId2);
        Task SaveChangesAsync();
    }
}