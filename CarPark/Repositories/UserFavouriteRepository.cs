using CarPark.Data;
using CarPark.Interfaces;
using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Repositories
{
    public class UserFavouriteRepository : IUserFavouriteRepository
    {
        private readonly AppDbContext _context;

        public UserFavouriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserFavourite>> GetByUserIdAsync(int userId)
            => await _context.UserFavourites
                .Include(f => f.CarPark)
                .Where(f => f.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

        public Task<UserFavourite?> GetAsync(int userId, int carParkId)
            => _context.UserFavourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CarParkId == carParkId);

        public async Task AddAsync(UserFavourite favourite)
            => await _context.UserFavourites.AddAsync(favourite);

        public Task RemoveAsync(UserFavourite favourite)
        {
            _context.UserFavourites.Remove(favourite);
            return Task.CompletedTask;
        }
    }
}