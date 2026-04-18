using System.Reflection;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Payments;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;
using SoulViet.Shared.Application.Common.Behaviors;
using StackExchange.Redis;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMarketplaceModule(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

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
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IMasterOrderRepository, MasterOrderRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(dbConnection);
                })
            );

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount * 5;
            });

            // Vnpay
            services.Configure<VnPayConfig>(options =>
            {
                var vnPaySection = configuration.GetSection("VnPay");

                options.TmnCode = Environment.ExpandEnvironmentVariables(vnPaySection["TmnCode"] ?? string.Empty);
                options.HashSecret = Environment.ExpandEnvironmentVariables(vnPaySection["HashSecret"] ?? string.Empty);
                options.BaseUrl = Environment.ExpandEnvironmentVariables(vnPaySection["BaseUrl"] ?? string.Empty);
                options.ReturnUrl = Environment.ExpandEnvironmentVariables(vnPaySection["ReturnUrl"] ?? string.Empty);

                options.Version = vnPaySection["Version"] ?? "2.1.0";
                options.Command = vnPaySection["Command"] ?? "pay";
                options.CurrencyCode = vnPaySection["CurrencyCode"] ?? "VND";
                options.Locale = vnPaySection["Locale"] ?? "vn";
            });
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IPaymentTimeoutService, PaymentTimeoutService>();
            services.AddHttpContextAccessor();

            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}