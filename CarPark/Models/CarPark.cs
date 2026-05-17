namespace CarPark.Models
{
    public class CarPark
    {
        public int Id { get; set; }
        public string CarParkNo { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal XCoord { get; set; }
        public decimal YCoord { get; set; }
        public string CarParkType { get; set; } = string.Empty;
        public string TypeOfParkingSystem { get; set; } = string.Empty;
        public string ShortTermParking { get; set; } = string.Empty;
        public bool FreeParking { get; set; }
        public bool NightParking { get; set; }
        public int CarParkDecks { get; set; }
        public decimal? GantryHeight { get; set; }
        public bool CarParkBasement { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<UserFavourite> Favourites { get; set; } = new List<UserFavourite>();
    }
}