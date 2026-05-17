using CarPark.Data;
using CarPark.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Repositories
{
    public class CarParkRepository : ICarParkRepository
    {
        private readonly AppDbContext _context;

        public CarParkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.CarPark>> GetAllAsync(CarParkFilter filter)
        {
            var query = _context.CarParks.AsQueryable();

            if (filter.FreeParking.HasValue)
                query = query.Where(cp => cp.FreeParking == filter.FreeParking.Value);

            if (filter.NightParking.HasValue)
                query = query.Where(cp => cp.NightParking == filter.NightParking.Value);

            if (filter.MinVehicleHeight.HasValue)
                query = query.Where(cp =>
                    cp.GantryHeight == null ||
                    cp.GantryHeight >= filter.MinVehicleHeight.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        public Task<Models.CarPark?> GetByCarParkNoAsync(string carParkNo)
            => _context.CarParks
                .AsNoTracking()
                .FirstOrDefaultAsync(cp => cp.CarParkNo == carParkNo);

        public async Task UpsertRangeAsync(IEnumerable<Models.CarPark> carParks)
        {
            foreach (var incoming in carParks)
            {
                var existing = await _context.CarParks
                    .FirstOrDefaultAsync(cp => cp.CarParkNo == incoming.CarParkNo);

                if (existing is null)
                {
                    incoming.CreatedAt = DateTime.UtcNow;
                    incoming.UpdatedAt = DateTime.UtcNow;
                    await _context.CarParks.AddAsync(incoming);
                }
                else
                {
                    existing.Address = incoming.Address;
                    existing.XCoord = incoming.XCoord;
                    existing.YCoord = incoming.YCoord;
                    existing.CarParkType = incoming.CarParkType;
                    existing.TypeOfParkingSystem = incoming.TypeOfParkingSystem;
                    existing.ShortTermParking = incoming.ShortTermParking;
                    existing.FreeParking = incoming.FreeParking;
                    existing.NightParking = incoming.NightParking;
                    existing.CarParkDecks = incoming.CarParkDecks;
                    existing.GantryHeight = incoming.GantryHeight;
                    existing.CarParkBasement = incoming.CarParkBasement;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}