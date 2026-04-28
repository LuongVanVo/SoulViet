using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface ISplitRoomRepository
{
    Task<SplitRoomSession?> GetRoomAsync(string roomId, CancellationToken cancellationToken = default);
    Task SetRoomAsync(SplitRoomSession room, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default);
}