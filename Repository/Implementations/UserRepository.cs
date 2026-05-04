using ChatAppMVC.Data;
using ChatAppMVC.Models;
using ChatAppMVC.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppMVC.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task<List<User>> GetAllUsersAsync()
{
    return await _context.Users.ToListAsync();
}

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}