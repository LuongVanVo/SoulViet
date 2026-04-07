using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;
using StackExchange.Redis;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMarketplaceModule(this IServiceCollection services, IConfiguration configuration)
        {
            var rawDbConn = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var dbConnection = Environment.ExpandEnvironmentVariables(rawDbConn ?? string.Empty);

            var redisConn = Environment.ExpandEnvironmentVariables(configuration["Redis:ConnectionString"] ?? "127.0.0.1:6381");

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConn);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "SoulViet_";
            });

            services.AddDbContext<MarketplaceDbContext>(options => options.UseNpgsql(dbConnection));

            // Register repositories, services, etc. here
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMarketplaceCategoryRepository, MarketplaceCategoryRepository>();
            services.AddScoped<IMarketplaceProductRepository, MarketplaceProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}