using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SoulViet.Modules.Social.Social.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSocialModule(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<SocialDbContext>(options => options.UseNpgsql(dbConnection));

            return services;
        }
    }
}