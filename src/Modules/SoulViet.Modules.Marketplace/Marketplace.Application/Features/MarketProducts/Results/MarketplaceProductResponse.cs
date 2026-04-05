using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;

public class MarketplaceProductResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int Stock { get; set; }
    public ProductType ProductType { get; set; }
}