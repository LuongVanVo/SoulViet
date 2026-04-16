using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories;
using SoulViet.Modules.Social.Social.Infrastructure.Services;
using SoulViet.Shared.Application.Common.Behaviors;
using System.Reflection;

namespace SoulViet.Modules.Social.Social.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSocialModule(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<SocialDbContext>(options => options.UseNpgsql(dbConnection));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostCommentRepository, PostCommentRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var assembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            return services;
        }
    }
}