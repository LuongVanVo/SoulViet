using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<MarketplaceCategoryDto>>
{
    
}