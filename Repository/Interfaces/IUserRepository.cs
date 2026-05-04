using ChatAppMVC.Models;

namespace ChatAppMVC.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task<List<User>> GetAllUsersAsync();
    }
}