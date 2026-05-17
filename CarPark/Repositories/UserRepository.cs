using CarPark.Data;
using CarPark.Interfaces;
using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByUsernameAsync(string username)
            => _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public Task<User?> GetByIdAsync(int id)
            => _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddAsync(User user)
            => await _context.Users.AddAsync(user);
    }
}