using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, MarketplaceCategoryResponse>
{
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    public DeleteCategoryHandler(IMarketplaceCategoryRepository marketplaceCategoryRepository)
    {
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
    }

    public async Task<MarketplaceCategoryResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Find the category by ID
        var category = await _marketplaceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new NotFoundException("Category not found");

        if (!category.IsActive)
            throw new BadRequestException("This category is already inactive");

        // Soft delete by setting IsActive to false
        await _marketplaceCategoryRepository.SoftDeleteAsync(category, cancellationToken);

        return new MarketplaceCategoryResponse
        {
            Success = true,
            Message = "Category deleted successfully",
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl ?? string.Empty,
        };
    }
}