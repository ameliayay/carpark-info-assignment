using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.CarPark> CarParks => Set<Models.CarPark>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserFavourite> UserFavourites => Set<UserFavourite>();
        public DbSet<BatchJobRecord> BatchJobRecords => Set<BatchJobRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── CarPark ──────────────────────────────────────
            modelBuilder.Entity<Models.CarPark>(e =>
            {
                e.HasKey(cp => cp.Id);
                e.HasIndex(cp => cp.CarParkNo).IsUnique();
                e.Property(cp => cp.CarParkNo).HasMaxLength(20).IsRequired();
                e.Property(cp => cp.Address).HasMaxLength(255).IsRequired();
                e.Property(cp => cp.CarParkType).HasMaxLength(50);
                e.Property(cp => cp.TypeOfParkingSystem).HasMaxLength(50);
                e.Property(cp => cp.ShortTermParking).HasMaxLength(50);
                e.Property(cp => cp.XCoord).HasPrecision(12, 4);
                e.Property(cp => cp.YCoord).HasPrecision(12, 4);
                e.Property(cp => cp.GantryHeight).HasPrecision(5, 2);

                // Indexes to speed up filter queries
                e.HasIndex(cp => cp.FreeParking);
                e.HasIndex(cp => cp.NightParking);
                e.HasIndex(cp => cp.GantryHeight);
            });

            // ── User ─────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Username).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Username).HasMaxLength(100).IsRequired();
                e.Property(u => u.Email).HasMaxLength(255).IsRequired();
                e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            });

            // ── UserFavourite ─────────────────────────────────
            modelBuilder.Entity<UserFavourite>(e =>
            {
                e.HasKey(f => f.Id);

                // One user cannot favourite same carpark twice
                e.HasIndex(f => new { f.UserId, f.CarParkId }).IsUnique();

                e.HasOne(f => f.User)
                 .WithMany(u => u.Favourites)
                 .HasForeignKey(f => f.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(f => f.CarPark)
                 .WithMany(cp => cp.Favourites)
                 .HasForeignKey(f => f.CarParkId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── BatchJobRecord ────────────────────────────────
            modelBuilder.Entity<BatchJobRecord>(e =>
            {
                e.HasKey(b => b.Id);
                e.HasIndex(b => b.FileName);
                e.Property(b => b.FileName).HasMaxLength(255).IsRequired();
                e.Property(b => b.ErrorMessage).HasMaxLength(2000);
            });
        }
    }
}