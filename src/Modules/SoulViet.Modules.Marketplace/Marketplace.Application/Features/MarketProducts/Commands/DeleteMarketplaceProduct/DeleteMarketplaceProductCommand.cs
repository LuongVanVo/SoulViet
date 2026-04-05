using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.DeleteMarketplaceProduct;

public class DeleteMarketplaceProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
}