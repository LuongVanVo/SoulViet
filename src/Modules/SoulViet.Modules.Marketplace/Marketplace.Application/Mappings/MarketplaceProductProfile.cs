using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.CreateMarketplaceProduct;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Mappings;

public class MarketplaceProductProfile : Profile
{
    public MarketplaceProductProfile()
    {
        CreateMap<ProductAttributeDto, ProductAttribute>().ReverseMap();
        CreateMap<ProductVariantDto, ProductVariant>().ReverseMap();

        CreateMap<CreateMarketplaceProductCommand, MarketProduct>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Media,
                opt => opt.MapFrom(src => new ProductMediaInfo
                {
                    MainImage = src.MainImage,
                    LandImages = src.LandImages,
                    VideoUrl = src.VideoUrl
                }));

        CreateMap<MarketProduct, MarketplaceProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src =>
                    src.MarketplaceCategory != null ? src.MarketplaceCategory.Name : string.Empty));


        CreateMap<UpdateMarketplaceProductCommand, MarketProduct>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PartnerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerifiedByAdmin, opt => opt.Ignore())

            .ForMember(dest => dest.Attributes, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore())

            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom((src, dest) => src.CategoryId ?? dest.CategoryId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom((src, dest) => src.Price ?? dest.Price))
            .ForMember(dest => dest.PromotionalPrice, opt => opt.MapFrom((src, dest) => src.PromotionalPrice ?? dest.PromotionalPrice))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom((src, dest) => src.Stock ?? dest.Stock))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom((src, dest) => src.ProductType ?? dest.ProductType))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive ?? dest.IsActive))

            .ForMember(dest => dest.Media, opt => opt.MapFrom((src, dest) => new ProductMediaInfo
            {
                MainImage = src.MainImage ?? dest.Media.MainImage,
                LandImages = src.LandImages ?? dest.Media.LandImages,
                VideoUrl = src.VideoUrl ?? dest.Media.VideoUrl
            }))

            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}