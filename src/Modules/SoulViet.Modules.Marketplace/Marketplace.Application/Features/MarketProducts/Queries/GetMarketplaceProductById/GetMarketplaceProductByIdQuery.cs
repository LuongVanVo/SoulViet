using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductById;

public class GetMarketplaceProductByIdQuery : IRequest<MarketplaceProductDto>
{
    public Guid Id { get; set; }
}