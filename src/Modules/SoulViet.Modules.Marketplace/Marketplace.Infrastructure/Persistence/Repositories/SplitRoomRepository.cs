using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class SplitRoomRepository : ISplitRoomRepository
{
    private readonly ICacheService _cacheService;
    private const string KeyPrefix = "SplitRoom:";
    public SplitRoomRepository(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<SplitRoomSession?> GetRoomAsync(string roomId, CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetAsync<SplitRoomSession>(KeyPrefix + roomId, cancellationToken);
    }

    public async Task SetRoomAsync(SplitRoomSession room, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var timeToLive = expiry ?? TimeSpan.FromMinutes(30);
        await _cacheService.SetAsync(KeyPrefix + room.RoomId, room, absoluteExpireTime: timeToLive, cancellationToken: cancellationToken);
    }

    public async Task DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveAsync(KeyPrefix + roomId, cancellationToken);
    }
}