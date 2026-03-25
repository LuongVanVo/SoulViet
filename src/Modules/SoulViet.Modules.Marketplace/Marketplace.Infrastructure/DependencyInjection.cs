using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMarketplaceModule(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<MarketplaceDbContext>(options => options.UseNpgsql(dbConnection));

            return services;
        }
    }
}