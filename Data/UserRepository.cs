using KovsieAssetTracker.Models;
using KovsieAssetTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KovsieAssetTracker.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.AsNoTracking().ToListAsync();

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Name == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user) =>
            await _context.Users.AddAsync(user);

        public void Update(User user) =>
            _context.Users.Update(user);

        public void Delete(User user) =>
            _context.Users.Remove(user);

        public async Task SaveAsync() =>
            await _context.SaveChangesAsync();
    }
}
