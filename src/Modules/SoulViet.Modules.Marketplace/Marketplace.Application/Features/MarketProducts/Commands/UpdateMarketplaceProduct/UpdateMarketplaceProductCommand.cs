using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;

public class UpdateMarketplaceProductCommand : IRequest<MarketplaceProductResponse>
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }

    public Guid? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public decimal? Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public int? Stock { get; set; }
    public Guid? ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
    public ProductType? ProductType { get; set; }

    public bool? IsActive { get; set; }

    public string? MainImage { get; set; }
    public List<string>? LandImages { get; set; }
    public string? VideoUrl { get; set; }

    public bool? HasVariants { get; set; }
    public List<ProductAttributeDto>? Attributes { get; set; }
    public List<ProductVariantDto>? Variants { get; set; }
}