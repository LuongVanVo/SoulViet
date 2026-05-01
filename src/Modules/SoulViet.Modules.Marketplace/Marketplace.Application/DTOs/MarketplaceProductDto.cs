using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class MarketplaceProductDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int Stock { get; set; }
    public Guid? ProvinceId { get; set; }
    public string? ProvinceName { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerifiedByAdmin { get; set; }

    public ProductMediaInfo Media { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }

    public bool HasVariants { get; set; }
    public ICollection<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
    public ICollection<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
}