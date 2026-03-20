using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using RabbitMQ.Client;

// Load Environment Variable
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Get configuration from env
var rawDbConn = builder.Configuration.GetConnectionString("DefaultConnection");
var dbConn = Environment.ExpandEnvironmentVariables(rawDbConn!);

var redisConn = Environment.ExpandEnvironmentVariables(builder.Configuration["Redis:ConnectionString"] ?? "127.0.0.1:6381");

var rmqHost = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:HostName"] ?? "localhost");
var rmqUser = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:UserName"] ?? "admin");
var rmqPass = Environment.ExpandEnvironmentVariables(builder.Configuration["RabbitMQ:Password"] ?? "admin123");
var rmqConn = $"amqp://{rmqUser}:{rmqPass}@{rmqHost}:5672";

// Register service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// health checck endpoint
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

var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
app.Run(backendUrl);