using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence;
using SoulViet.Shared.Infrastructure.Persistence;

namespace SoulViet.API
{
    public class AppMigrationDbContext : DbContext
    {
        public AppMigrationDbContext(DbContextOptions<AppMigrationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketplaceDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SoulMapDbContext).Assembly);

        }
    }
}