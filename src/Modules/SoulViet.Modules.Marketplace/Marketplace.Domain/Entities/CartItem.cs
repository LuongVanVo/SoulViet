using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class CartItem : BaseAuditableEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid MarketplaceProductId { get; set; }
    public MarketProduct MarketplaceProduct { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ItemMetadata { get; set; }
    public Guid? VariantId { get; set; }
    public ProductVariant? Variant { get; set; }
}