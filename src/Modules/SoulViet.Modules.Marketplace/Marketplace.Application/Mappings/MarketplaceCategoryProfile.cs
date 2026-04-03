using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Mappings;

public class MarketplaceCategoryProfile : Profile
{
    public MarketplaceCategoryProfile()
    {
        CreateMap<CreateCategoryCommand, MarketplaceCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        // Mapping from Entity to DTO
        CreateMap<MarketplaceCategory, MarketplaceCategoryDto>();
    }
}