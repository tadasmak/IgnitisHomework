using Microsoft.EntityFrameworkCore;
using IgnitisHomework.Models;

namespace IgnitisHomework.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PowerPlant> PowerPlants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PowerPlant>().HasData(
                new PowerPlant { Id = 1, Owner = "Vardenis Pavardenis", Power = 9.3, ValidFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), ValidTo = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PowerPlant { Id = 2, Owner = "Jonas Jonaitis", Power = 5.7, ValidFrom = new DateTime(2021, 6, 15, 0, 0, 0, DateTimeKind.Utc), ValidTo = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc) },
                new PowerPlant { Id = 3, Owner = "Ona Petraitë", Power = 12.5, ValidFrom = new DateTime(2019, 9, 10, 0, 0, 0, DateTimeKind.Utc), ValidTo = null }
            );
        }
    }
}