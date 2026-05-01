namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int Stock { get; set; }
    public string AttributesJson { get; set; } = "{}";
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}