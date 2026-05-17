using CarPark.DTOs;
using CarPark.Interfaces;

namespace CarPark.Services
{
    public interface ICarParkService
    {
        Task<IEnumerable<CarParkDto>> GetCarParksAsync(CarParkFilter filter);
        Task<CarParkDto?> GetByCarParkNoAsync(string carParkNo);
    }

    public class CarParkService : ICarParkService
    {
        private readonly ICarParkRepository _repo;

        public CarParkService(ICarParkRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CarParkDto>> GetCarParksAsync(CarParkFilter filter)
        {
            var carParks = await _repo.GetAllAsync(filter);

            return carParks.Select(cp => new CarParkDto(
                cp.Id,
                cp.CarParkNo,
                cp.Address,
                cp.XCoord,
                cp.YCoord,
                cp.CarParkType,
                cp.TypeOfParkingSystem,
                cp.ShortTermParking,
                cp.FreeParking,
                cp.NightParking,
                cp.CarParkDecks,
                cp.GantryHeight,
                cp.CarParkBasement
            ));
        }

        public async Task<CarParkDto?> GetByCarParkNoAsync(string carParkNo)
        {
            var cp = await _repo.GetByCarParkNoAsync(carParkNo);
            if (cp is null) return null;

            return new CarParkDto(
                cp.Id,
                cp.CarParkNo,
                cp.Address,
                cp.XCoord,
                cp.YCoord,
                cp.CarParkType,
                cp.TypeOfParkingSystem,
                cp.ShortTermParking,
                cp.FreeParking,
                cp.NightParking,
                cp.CarParkDecks,
                cp.GantryHeight,
                cp.CarParkBasement
            );
        }
    }
}