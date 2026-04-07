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
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.MarketplaceProduct.Name))
            .ForMember(dest => dest.MainImage,
                opt => opt.MapFrom(src => src.MarketplaceProduct.Media.MainImage))
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.MarketplaceProduct.Price))
            .ForMember(dest => dest.PromotionalPrice,
                opt => opt.MapFrom(src => src.MarketplaceProduct.PromotionalPrice))
            .ForMember(dest => dest.Stock,
                opt => opt.MapFrom(src => src.MarketplaceProduct.Stock))
            .ForMember(dest => dest.PartnerId,
                opt => opt.MapFrom(src => src.MarketplaceProduct.PartnerId));
    }
}