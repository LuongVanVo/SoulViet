using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.FinalizeSplitOrder;

public class FinalizeSplitOrderCommand : IRequest<bool>
{
    public string RoomId { get; set; } = String.Empty;
}