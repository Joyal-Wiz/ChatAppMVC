using ChatAppMVC.Data;
using ChatAppMVC.Models;
using ChatAppMVC.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppMVC.Repository.Implementations
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task<List<Message>> GetMessagesAsync(int userId1, int userId2)
        {
            return await _context.Messages
                .Include(m => m.Sender)   // ✅ FIX
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1)
                )
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> GetLastMessageAsync(int userId1, int userId2)
        {
            return await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1)
                )
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}