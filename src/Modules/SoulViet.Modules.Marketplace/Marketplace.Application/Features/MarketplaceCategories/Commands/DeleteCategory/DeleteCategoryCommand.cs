using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<MarketplaceCategoryResponse>
{
    public Guid Id { get; set; }
}