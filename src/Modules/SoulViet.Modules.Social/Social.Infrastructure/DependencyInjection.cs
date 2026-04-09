using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories;
using System.Reflection;
using MediatR;

namespace SoulViet.Modules.Social.Social.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSocialModule(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<SocialDbContext>(options => options.UseNpgsql(dbConnection));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var assembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            return services;
        }
    }
}