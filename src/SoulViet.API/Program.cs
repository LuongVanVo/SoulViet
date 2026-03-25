using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using RabbitMQ.Client;

// Import Dependency Injection Extensions from Modules
using SoulViet.Shared.Infrastructure;
using SoulViet.Modules.Social.Social.Infrastructure;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure;
using SoulViet.API;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Seeder;

// Load Environment Variable
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Get configuration from env
var rawDbConn = builder.Configuration.GetConnectionString("DefaultConnection");

var dbConn = rawDbConn!
    .Replace("%DB_HOST%", Environment.GetEnvironmentVariable("DB_HOST"))
    .Replace("%DB_PORT%", Environment.GetEnvironmentVariable("DB_PORT"))
    .Replace("%DB_NAME%", Environment.GetEnvironmentVariable("DB_NAME"))
    .Replace("%DB_USER%", Environment.GetEnvironmentVariable("DB_USER"))
    .Replace("%DB_PASSWORD%", Environment.GetEnvironmentVariable("DB_PASSWORD"));

var redisConn = Environment.ExpandEnvironmentVariables(builder.Configuration["Redis:ConnectionString"] ?? "127.0.0.1:6381");

var rmqHost = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:HostName"] ?? "localhost");
var rmqUser = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:UserName"] ?? "admin");
var rmqPass = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:Password"] ?? "admin123");
var rmqConn = $"amqp://{rmqUser}:{rmqPass}@{rmqHost}:5672";

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- REGISTER DEPENDENCY INJECTION MODULES  ---
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSocialModule(builder.Configuration);
builder.Services.AddMarketplaceModule(builder.Configuration);
builder.Services.AddSoulMapModule(builder.Configuration);

builder.Services.AddDbContext<AppMigrationDbContext>(options =>
{
    options.UseNpgsql(dbConn, o => o.UseNetTopologySuite());
});

// Health check api
builder.Services.AddHealthChecks()
    .AddNpgSql(dbConn, name: "PostgreSQL Database")
    .AddRedis(redisConn, name: "Redis Cache")
    .AddRabbitMQ(sp => {
        var factory = new ConnectionFactory { Uri = new Uri(rmqConn) };
        return factory.CreateConnectionAsync();
    }, name: "RabbitMQ Message Broker");

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Root
app.MapGet("/", () => new
{
    Message = "SoulViet API v1.0",
    Status = "Running",
    Environment = app.Environment.EnvironmentName
})
.WithName("Root");

// health check endpoint
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(), 
            TotalDuration = report.TotalDuration.ToString(),
            Components = report.Entries.Select(e => new
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Error = e.Value.Exception?.Message
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        var env = services.GetRequiredService<IWebHostEnvironment>();

        logger.LogInformation("Starting database migration and seeding...");

        var rootDir = Directory.GetParent(env.ContentRootPath)?.Parent?.FullName;
        var dataFolder = Path.Combine(rootDir ?? env.ContentRootPath, "data");

        var touristPath = Path.Combine(dataFolder, "SoulViet_DiaDiemDuLich.csv");
        var accommodationPath = Path.Combine(dataFolder, "SoulViet_ChoO.csv");

        // var soulMapSeeder = services.GetRequiredService<SoulMapDataSeeder>();
        // await soulMapSeeder.SeedDataAsync(touristPath, accommodationPath);

        logger.LogInformation("Database migration and seeding completed successfully.");
    } 
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
app.Run(backendUrl);