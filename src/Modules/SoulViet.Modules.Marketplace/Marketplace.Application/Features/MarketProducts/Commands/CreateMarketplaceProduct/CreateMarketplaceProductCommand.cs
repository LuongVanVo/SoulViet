using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.CreateMarketplaceProduct;

public class CreateMarketplaceProductCommand : IRequest<MarketplaceProductResponse>
{
    public Guid PartnerId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int Stock { get; set; }
    public Guid? ProvinceId { get; set; }
    public string? ProvinceName { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }

    public string MainImage { get; set; } = string.Empty;
    public List<string> LandImages { get; set; } = new();
    public string VideoUrl { get; set; } = string.Empty;
}