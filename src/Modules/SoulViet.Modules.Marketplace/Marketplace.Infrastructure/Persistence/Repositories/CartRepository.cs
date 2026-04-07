// File: CartRepository.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Shared.Application.Interfaces;
using StackExchange.Redis;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly MarketplaceDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CartRepository> _logger;
    private readonly IMapper _mapper;

    public CartRepository(MarketplaceDbContext dbContext, IConnectionMultiplexer redis, IBackgroundTaskQueue taskQueue, IServiceScopeFactory scopeFactory, ILogger<CartRepository> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _redis = redis;
        _taskQueue = taskQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _mapper = mapper;
    }

    // Tiền tố cố định, thay thế hoàn toàn được ICacheService
    private static string GetRedisKey(Guid userId) => $"SoulViet_marketplace:cart:{userId}";

    // B1: Đọc - Ưu tiên load từ Redis ra trước để giảm tải DB
    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var cachedJson = await db.StringGetAsync(GetRedisKey(userId));

        if (!cachedJson.IsNullOrEmpty)
        {
            var options = new JsonSerializerOptions 
            { 
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true 
            };
            var cartFromRedis = JsonSerializer.Deserialize<Cart>(cachedJson.ToString(), options);
            if (cartFromRedis != null) return cartFromRedis;
        }

        // Nếu Redis ko có, xuống PostgreSQL quét lên
        return await _dbContext.Carts
            .Include(x => x.Items)
                .ThenInclude(i => i.MarketplaceProduct)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    // B2: Ghi dữ liệu
    public async Task SaveCartAsync(Guid userId, Cart cart, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var jsonCart = JsonSerializer.Serialize(cart, jsonOptions);

        await db.StringSetAsync(GetRedisKey(userId), jsonCart, TimeSpan.FromDays(30));

        await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
        {
            await SyncToPostgresBackgroundAsync(userId, cart, token);
        });
    }

    // B3: Xóa giỏ hàng an toàn
    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(GetRedisKey(userId)); // Xoá Cache

        var dbCart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (dbCart != null)
        {
            _dbContext.Carts.Remove(dbCart);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _dbContext.Carts.AddAsync(cart, cancellationToken);
    }
    public Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private async Task SyncToPostgresBackgroundAsync(Guid userId, Cart cart, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

            var dbCart = await dbContext.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (dbCart == null)
            {
                dbCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                };
                dbContext.Carts.Add(dbCart);
                await dbContext.SaveChangesAsync(cancellationToken);
            } 
            else
            {
                dbCart.LastModifiedAt = DateTime.UtcNow;
            }

            var existingDbItems = dbCart.Items.ToDictionary(i => i.Id);

            foreach (var item in cart.Items)
            {
                if (existingDbItems.TryGetValue(item.Id, out var dbItem))
                {
                    dbItem.Quantity = item.Quantity;
                    dbItem.ItemMetadata = item.ItemMetadata;
                    existingDbItems.Remove(item.Id);
                }
                else
                {
                    var newItem = _mapper.Map<CartItem>(item);
                    newItem.CartId = dbCart.Id;
                    newItem.MarketplaceProduct = null!;
                    newItem.Cart = null!;

                    if (newItem.Id == Guid.Empty) newItem.Id = Guid.NewGuid();
                    dbContext.CartItems.Add(newItem);
                }
            }

            if (existingDbItems.Any())
            {
                dbContext.CartItems.RemoveRange(existingDbItems.Values);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully background-synced cart for user {UserId} to PostgreSQL", userId);
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing cart for user {UserId} to PostgreSQL via Background Task", userId);
        }
    }
}