using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMarketplaceModule(this IServiceCollection services, IConfiguration configuration)
        {
            var rawDbConn = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var dbConnection = Environment.ExpandEnvironmentVariables(rawDbConn ?? string.Empty);

            services.AddDbContext<MarketplaceDbContext>(options => options.UseNpgsql(dbConnection));

            // Register repositories, services, etc. here
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMarketplaceCategoryRepository, MarketplaceCategoryRepository>();
            services.AddScoped<IMarketplaceProductRepository, MarketplaceProductRepository>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}