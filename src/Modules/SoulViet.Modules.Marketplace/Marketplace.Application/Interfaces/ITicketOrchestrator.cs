using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Shared.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

public interface ITicketOrchestrator
{
    Task<List<TicketEmailInfo>> ProcessTicketsForOrderAsync(Guid masterOrderId, CancellationToken cancellationToken = default);
}