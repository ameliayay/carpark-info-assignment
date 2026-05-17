namespace CarPark.Models
{
    public class UserFavourite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CarParkId { get; set; }
        public CarPark CarPark { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
