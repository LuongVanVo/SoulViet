using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Seeder;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSoulMapModule(this IServiceCollection services, IConfiguration configuration)
        {
            var rawDbConn = configuration.GetConnectionString("DefaultConnection");
        
            var dbConnection = rawDbConn!
                .Replace("%DB_HOST%", Environment.GetEnvironmentVariable("DB_HOST"))
                .Replace("%DB_PORT%", Environment.GetEnvironmentVariable("DB_PORT"))
                .Replace("%DB_NAME%", Environment.GetEnvironmentVariable("DB_NAME"))
                .Replace("%DB_USER%", Environment.GetEnvironmentVariable("DB_USER"))
                .Replace("%DB_PASSWORD%", Environment.GetEnvironmentVariable("DB_PASSWORD"));

            services.AddDbContext<SoulMapDbContext>(options => 
                options.UseNpgsql(dbConnection, o => o.UseNetTopologySuite())
            );

            services.AddScoped<SoulMapDataSeeder>();

            return services;
        }
    }
}