using CarPark.Models;

namespace CarPark.Interfaces
{
    public class CarParkFilter
    {
        public bool? FreeParking { get; set; }
        public bool? NightParking { get; set; }
        public decimal? MinVehicleHeight { get; set; }
    }

    public interface ICarParkRepository
    {
        Task<IEnumerable<Models.CarPark>> GetAllAsync(CarParkFilter filter);
        Task<Models.CarPark?> GetByCarParkNoAsync(string carParkNo);
        Task UpsertRangeAsync(IEnumerable<Models.CarPark> carParks);
    }

    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
    }

    public interface IUserFavouriteRepository
    {
        Task<IEnumerable<UserFavourite>> GetByUserIdAsync(int userId);
        Task<UserFavourite?> GetAsync(int userId, int carParkId);
        Task AddAsync(UserFavourite favourite);
        Task RemoveAsync(UserFavourite favourite);
    }

    public interface IBatchJobRepository
    {
        Task<BatchJobRecord?> GetByFileNameAsync(string fileName);
        Task AddAsync(BatchJobRecord record);
        Task UpdateAsync(BatchJobRecord record);
    }
}