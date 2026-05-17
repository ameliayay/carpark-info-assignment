using CarPark.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.Globalization;

namespace CarPark.Batch
{
    public class CsvCarParkParser
    {
        public async Task<List<Models.CarPark>> ParseAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null
            };

            var carParks = new List<Models.CarPark>();

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var rawFreeParking = csv.GetField("free_parking") ?? "NO";
                var rawNightParking = csv.GetField("night_parking") ?? "NO";
                var rawBasement = csv.GetField("car_park_basement") ?? "N";
                var rawGantryHeight = csv.GetField("gantry_height") ?? "0";

                // Parse gantry height — 0 means no restriction, store as null
                decimal.TryParse(rawGantryHeight, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var gantryHeight);

                carParks.Add(new Models.CarPark
                {
                    CarParkNo = csv.GetField("car_park_no") ?? string.Empty,
                    Address = csv.GetField("address") ?? string.Empty,
                    XCoord = decimal.Parse(
                        csv.GetField("x_coord") ?? "0",
                        CultureInfo.InvariantCulture),
                    YCoord = decimal.Parse(
                        csv.GetField("y_coord") ?? "0",
                        CultureInfo.InvariantCulture),
                    CarParkType = csv.GetField("car_park_type") ?? string.Empty,
                    TypeOfParkingSystem = csv.GetField("type_of_parking_system") ?? string.Empty,
                    ShortTermParking = csv.GetField("short_term_parking") ?? string.Empty,

                    // Anything that is not "NO" means free parking is available
                    FreeParking = !rawFreeParking.Equals("NO", StringComparison.OrdinalIgnoreCase),

                    // YES means night parking available
                    NightParking = rawNightParking.Equals("YES", StringComparison.OrdinalIgnoreCase),

                    // 0 means no restriction → store as null
                    GantryHeight = gantryHeight == 0 ? null : gantryHeight,

                    CarParkDecks = int.TryParse(csv.GetField("car_park_decks"), out var decks) ? decks : 0,

                    // Y means basement carpark
                    CarParkBasement = rawBasement.Equals("Y", StringComparison.OrdinalIgnoreCase),

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return carParks;
        }
    }
}