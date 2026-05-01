using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class ProductAttribute : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public MarketProduct Product { get; set; } = null!;

    public string Name { get; set; } = string.Empty; // e.g., "Color", "Size"
    public string OptionsJson { get; set; } = "[]"; // JSON array of options, e.g., ["Red", "Blue", "Green"] for Color
    public int SortOrder { get; set; } = 0;
}