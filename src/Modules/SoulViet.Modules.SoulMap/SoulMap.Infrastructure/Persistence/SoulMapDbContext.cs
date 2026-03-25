using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence
{
    public class SoulMapDbContext : DbContext
    {
        public SoulMapDbContext(DbContextOptions<SoulMapDbContext> options) : base(options)
        {
            
        }

        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<TouristAttraction> TouristAttractions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.HasDefaultSchema("soulmap");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}