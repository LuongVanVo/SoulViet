using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Cart, CartDto>();

        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MarketplaceProductId,
                opt => opt.MapFrom(src => src.MarketplaceProductId))

            .ForMember(dest => dest.VariantId,
                opt => opt.MapFrom(src => src.VariantId))
            .ForMember(dest => dest.VariantAttributesJson,
                opt => opt.MapFrom(src => src.Variant != null ? src.Variant.AttributesJson : null))

            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.MarketplaceProduct.Name))

            .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src =>
                src.Variant != null && !string.IsNullOrEmpty(src.Variant.ImageUrl)
                    ? src.Variant.ImageUrl
                    : src.MarketplaceProduct.Media.MainImage))

            .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                src.Variant != null ? src.Variant.Price : src.MarketplaceProduct.Price))

            .ForMember(dest => dest.PromotionalPrice, opt => opt.MapFrom(src =>
                src.Variant != null ? src.Variant.PromotionalPrice : src.MarketplaceProduct.PromotionalPrice))

            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src =>
                src.Variant != null ? src.Variant.Stock : src.MarketplaceProduct.Stock))

            .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.MarketplaceProduct.PartnerId));
    }
}