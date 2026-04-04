using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.UpdateCategory;
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

        // Mapping Update Category Command to Entity
        CreateMap<UpdateCategoryCommand, MarketplaceCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryType, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
            {
                // Bỏ qua nếu là chuỗi rỗng hoặc null
                if (srcMember is string strValue)
                {
                    return !string.IsNullOrWhiteSpace(strValue);
                }

                return srcMember != null;
            }));
    }
}