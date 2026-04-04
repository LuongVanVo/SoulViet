using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<MarketplaceCategoryDto>
{
    public Guid Id { get; set; }
}