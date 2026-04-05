using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.VerifyMarketProduct;

public class VerifyMarketProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}