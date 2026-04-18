using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SoulViet.Shared.Application.Common.ExternalSettings;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Infrastructure.Authentication;
using SoulViet.Shared.Infrastructure.Consumer;
using SoulViet.Shared.Infrastructure.Persistence;
using SoulViet.Shared.Infrastructure.Persistence.Repositories;
using SoulViet.Shared.Infrastructure.Services;

namespace SoulViet.Shared.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConn = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(defaultConn) || defaultConn.Contains('%'))
            {
                defaultConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                              ?? configuration["DB_CONNECTION_STRING"];
            }

            var dbConnection = Environment.ExpandEnvironmentVariables(defaultConn ?? string.Empty);

            services.AddDbContext<SharedDbContext>(options => options.UseNpgsql(dbConnection));

            // Config redis cache
            var redisConn =
                Environment.ExpandEnvironmentVariables(configuration["Redis:ConnectionString"] ?? "127.0.0.1:6381");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "SoulViet_";
            });

            services.AddTransient<ICacheService, CacheService>();

            // Register repositories and other services here
            services.AddHttpContextAccessor();
            services.AddSingleton<IJwtKeyProvider, RsaKeyProvider>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICookieService, CookieService>();

            var serviceProvider = services.BuildServiceProvider();
            var rsaKeyProvider = serviceProvider.GetRequiredService<IJwtKeyProvider>();

            // config RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendEmailConsumer>();
                x.AddConsumer<ForgotPasswordConsumer>();
                x.AddConsumer<UserOrderCreatedConsumer>();
                x.AddConsumer<PartnerOrderCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Connect to RabbitMQ Docker
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER" ) ?? "admin");
                        h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASS" ) ?? "admin123");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            var rawMailSettings = configuration.GetSection("MailSettings").Get<MailSettings>() ?? new MailSettings();

            var mailSettings = new MailSettings
            {
                Server = Environment.ExpandEnvironmentVariables(rawMailSettings.Server ?? string.Empty),
                Port = rawMailSettings.Port, // Giữ nguyên vì là số 587
                SenderName = Environment.ExpandEnvironmentVariables(rawMailSettings.SenderName ?? string.Empty),
                SenderEmail = Environment.ExpandEnvironmentVariables(rawMailSettings.SenderEmail ?? string.Empty),
                Password = Environment.ExpandEnvironmentVariables(rawMailSettings.Password ?? string.Empty)
            };

            services.AddSingleton(mailSettings);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Bearer";
                    options.DefaultChallengeScheme = "Bearer";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(rsaKeyProvider.GetPublicKey()),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Cookies["access_token"];
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddAuthorization();

            // Register Email Service
            services.AddTransient<IEmailService, EmailService>();
            // Register User Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            services.AddSingleton<IBackgroundTaskQueue>(ctx => new BackgroundTaskQueue(capacity: 200));
            services.AddHostedService<QueuedHostedService>();

            return services;
        } 
    }
}