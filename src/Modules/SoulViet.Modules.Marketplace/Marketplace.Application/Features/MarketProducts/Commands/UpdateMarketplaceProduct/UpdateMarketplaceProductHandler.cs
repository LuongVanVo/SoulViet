using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;

public class  UpdateMarketplaceProductHandler : IRequestHandler<UpdateMarketplaceProductCommand, MarketplaceProductResponse>
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
        var product = await _marketplaceProductRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
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

        if (request.HasVariants.HasValue && request.HasVariants.Value == true)
        {
            // Delete all existing variants and attributes if the product is updated to have variants
            product.Attributes.Clear();
            product.Variants.Clear();

            // Add new variants and attributes from the request
            if (request.Attributes != null)
            {
                foreach (var attrDto in request.Attributes)
                {
                    var newAttr = _mapper.Map<ProductAttribute>(attrDto);
                    newAttr.ProductId = product.Id;
                    product.Attributes.Add(newAttr);
                }
            }

            if (request.Variants != null)
            {
                foreach (var variantDto in request.Variants)
                {
                    var newVariant = _mapper.Map<ProductVariant>(variantDto);
                    newVariant.ProductId = product.Id;
                    product.Variants.Add(newVariant);
                }
            }

            if (product.Variants.Any())
            {
                product.Price = product.Variants.Min(v => v.Price);

                var promotionalPrices = product.Variants
                    .Where(v => v.PromotionalPrice.HasValue)
                    .Select(v => v.PromotionalPrice.Value);

                product.PromotionalPrice = promotionalPrices.Any() ? promotionalPrices.Min() : null;
                product.Stock = product.Variants.Sum(v => v.Stock);
            }
        }
        else if (request.HasVariants.HasValue && request.HasVariants.Value == false)
        {
            product.Attributes.Clear();
            product.Variants.Clear();
            if(request.Price != 0) product.Price = request.Price ?? product.Price;
            product.PromotionalPrice = request.PromotionalPrice;
            if(request.Stock != 0) product.Stock = request.Stock ?? product.Stock;
        }

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