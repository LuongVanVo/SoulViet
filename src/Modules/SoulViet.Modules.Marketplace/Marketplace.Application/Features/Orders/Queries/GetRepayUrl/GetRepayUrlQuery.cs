using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetRepayUrl;

public class GetRepayUrlQuery : IRequest<string>
{
    public Guid MasterOrderId { get; set; }
    public Guid UserId { get; set; }
}