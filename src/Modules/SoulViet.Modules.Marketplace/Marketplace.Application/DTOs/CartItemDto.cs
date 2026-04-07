namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid MarketplaceProductId { get; set; }

    // Info product
    public string ProductName { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int Stock { get; set; }

    // This field is used grouping cart items by partner when checkout, to create separate orders for each partner
    public Guid PartnerId { get; set; }

    // Info cart item
    public int Quantity { get; set; }
    public string? ItemMetadata { get; set; } // Json contains additional info for the cart item, such as selected options, tour date, etc.

    // Calculated fields
    public decimal SubTotal => Quantity * (PromotionalPrice ?? Price);
}