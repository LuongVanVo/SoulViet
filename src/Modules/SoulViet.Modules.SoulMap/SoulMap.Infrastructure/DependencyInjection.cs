using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesInBounds;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Seeder;
using SoulViet.Modules.SoulMap.SoulMap.Application.Services;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSoulMapModule(this IServiceCollection services, IConfiguration configuration)
        {
            var rawDbConn = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var dbConnection = Environment.ExpandEnvironmentVariables(rawDbConn ?? string.Empty);

            services.AddDbContext<SoulMapDbContext>(options => 
                options.UseNpgsql(dbConnection, o => o.UseNetTopologySuite())
            );

            var applicationAssembly = typeof(GetPlacesInBoundsHandler).Assembly;

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);

                // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddScoped<SoulMapDataSeeder>();
            services.AddScoped<MediaMigrationService>();
            services.AddScoped<IMapRepository, MapRepository>();

            return services;
        }
    }
}