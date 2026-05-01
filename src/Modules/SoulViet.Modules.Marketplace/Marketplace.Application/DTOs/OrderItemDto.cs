namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public string? VariantNameSnapshot { get; set; }
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public string ProductImageSnapshot { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ItemMetadata { get; set; }
}