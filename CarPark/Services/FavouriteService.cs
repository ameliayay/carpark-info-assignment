using CarPark.DTOs;
using CarPark.Interfaces;
using CarPark.Models;

namespace CarPark.Services
{
    public interface IFavouriteService
    {
        Task<IEnumerable<FavouriteDto>> GetUserFavouritesAsync(int userId);
        Task<bool> AddFavouriteAsync(int userId, string carParkNo);
        Task<bool> RemoveFavouriteAsync(int userId, string carParkNo);
    }

    public class FavouriteService : IFavouriteService
    {
        private readonly IUserFavouriteRepository _favouriteRepo;
        private readonly ICarParkRepository _carParkRepo;
        private readonly IUnitOfWork _uow;

        public FavouriteService(
            IUserFavouriteRepository favouriteRepo,
            ICarParkRepository carParkRepo,
            IUnitOfWork uow)
        {
            _favouriteRepo = favouriteRepo;
            _carParkRepo = carParkRepo;
            _uow = uow;
        }

        public async Task<IEnumerable<FavouriteDto>> GetUserFavouritesAsync(int userId)
        {
            var favs = await _favouriteRepo.GetByUserIdAsync(userId);
            return favs.Select(f => new FavouriteDto(
                f.CarParkId,
                f.CarPark.CarParkNo,
                f.CarPark.Address,
                f.SavedAt
            ));
        }

        public async Task<bool> AddFavouriteAsync(int userId, string carParkNo)
        {
            var carPark = await _carParkRepo.GetByCarParkNoAsync(carParkNo);
            if (carPark is null) return false;

            // Idempotent — if already favourited, just return true
            var existing = await _favouriteRepo.GetAsync(userId, carPark.Id);
            if (existing is not null) return true;

            var favourite = new UserFavourite
            {
                UserId = userId,
                CarParkId = carPark.Id,
                SavedAt = DateTime.UtcNow
            };

            await _favouriteRepo.AddAsync(favourite);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavouriteAsync(int userId, string carParkNo)
        {
            var carPark = await _carParkRepo.GetByCarParkNoAsync(carParkNo);
            if (carPark is null) return false;

            var favourite = await _favouriteRepo.GetAsync(userId, carPark.Id);
            if (favourite is null) return false;

            await _favouriteRepo.RemoveAsync(favourite);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}