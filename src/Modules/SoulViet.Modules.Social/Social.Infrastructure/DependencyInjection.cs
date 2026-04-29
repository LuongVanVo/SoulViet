using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Infrastructure.Consumer;
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
            var defaultConn = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(defaultConn) || defaultConn.Contains('%'))
            {
                defaultConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                              ?? configuration["DB_CONNECTION_STRING"];
            }

            var dbConnection = Environment.ExpandEnvironmentVariables(defaultConn ?? string.Empty);

            services.AddDbContext<SocialDbContext>(options => options.UseNpgsql(dbConnection));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostCommentRepository, PostCommentRepository>();
            services.AddScoped<IPostLikeRepository, PostLikeRepository>();
            services.AddScoped<IPostShareRepository, PostShareRepository>();
            services.AddScoped<IComboExperienceRepository, ComboExperienceRepository>();
            services.AddScoped<IUserFollowerRepository, UserFollowerRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISoulMapService, SoulMapService>();

            services.AddSingleton<SseConnectionManager>();
            services.AddSingleton<PostCommentEventService>();
            services.AddSingleton<ICommentEventService>(sp => sp.GetRequiredService<PostCommentEventService>());

            services.AddSingleton<PostShareEventService>();
            services.AddSingleton<IShareEventService>(sp => sp.GetRequiredService<PostShareEventService>());

            var infrastructureAssembly = Assembly.GetExecutingAssembly();
            var applicationAssembly = typeof(Social.Application.Features.Posts.Commands.CreatePost.CreatePostCommand).Assembly;

            services.AddAutoMapper(cfg => 
            {
                cfg.AddMaps(infrastructureAssembly);
                cfg.AddMaps(applicationAssembly);
            });

            services.AddValidatorsFromAssembly(applicationAssembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
                cfg.AddOpenBehavior(typeof(Social.Application.Common.Behaviors.ValidationBehavior<,>));
            });
            return services;
        }
    }
}