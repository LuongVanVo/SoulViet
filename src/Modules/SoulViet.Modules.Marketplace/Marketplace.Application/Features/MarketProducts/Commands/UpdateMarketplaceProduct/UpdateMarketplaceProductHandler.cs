using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;

public class UpdateMarketplaceProductHandler : IRequestHandler<UpdateMarketplaceProductCommand, MarketplaceProductResponse>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateMarketplaceProductHandler(IMarketplaceProductRepository marketplaceProductRepository, IMarketplaceCategoryRepository marketplaceCategoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<MarketplaceProductResponse> Handle(UpdateMarketplaceProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _marketplaceProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException("Product not found");

        if (product.PartnerId != request.PartnerId)
            throw new ForbiddenException("You do not have permission to update this product");

        if ((request.CategoryId.HasValue && product.CategoryId != request.CategoryId.Value) ||
            (request.ProductType.HasValue && product.ProductType != request.ProductType.Value))
        {
            var categoryIdToCheck = request.CategoryId ?? product.CategoryId;
            var category = await _marketplaceCategoryRepository.GetByIdAsync(categoryIdToCheck, cancellationToken);

            if (category == null || !category.IsActive)
                throw new BadRequestException("Category not exists or is inactive");

            var productTypeToCheck = request.ProductType ?? product.ProductType;
            if (productTypeToCheck != category.CategoryType)
                throw new BadRequestException("Product type does not match category type");
        }

        _mapper.Map(request, product);

        await _marketplaceProductRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MarketplaceProductResponse
        {
            Success = true,
            Message = "Product updated successfully",
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ProductType = product.ProductType,
            PromotionalPrice = product.PromotionalPrice,
            Stock = product.Stock
        };
    }
}