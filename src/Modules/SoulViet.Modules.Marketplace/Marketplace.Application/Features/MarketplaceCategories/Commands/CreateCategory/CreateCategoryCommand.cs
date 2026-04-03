using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<MarketplaceCategoryResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public ProductType CategoryType { get; set; }
}