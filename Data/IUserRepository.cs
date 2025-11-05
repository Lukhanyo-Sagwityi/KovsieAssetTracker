using KovsieAssetTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KovsieAssetTracker.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email); // NEW
        Task<User?> GetByUserNameAsync(string username);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
        Task SaveAsync();
    }
}
