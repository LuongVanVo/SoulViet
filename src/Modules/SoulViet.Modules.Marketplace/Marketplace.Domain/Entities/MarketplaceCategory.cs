using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class MarketplaceCategory : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public ProductType CategoryType { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<MarketProduct> Products { get; set; } = new List<MarketProduct>();
}