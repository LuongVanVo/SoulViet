using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<bool>
{
    public Guid MasterOrderId { get; set; }
    public Guid UserId { get; set; }
}