using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.CreateMarketplaceProduct;

public class CreateMarketplaceProductHandler : IRequestHandler<CreateMarketplaceProductCommand, MarketplaceProductResponse>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public CreateMarketplaceProductHandler(IMarketplaceProductRepository marketplaceProductRepository, IMarketplaceCategoryRepository marketplaceCategoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<MarketplaceProductResponse> Handle(CreateMarketplaceProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _marketplaceCategoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null || !category.IsActive)
            throw new BadRequestException("Category not exists or is inactive");

        if (request.ProductType != category.CategoryType)
            throw new BadRequestException("Product type does not match category type");

        // mapping
        var product = _mapper.Map<MarketProduct>(request);
        product.Id = Guid.NewGuid();

        if (product.HasVariants)
        {
            if (!product.Attributes.Any() || !product.Variants.Any())
                throw new BadRequestException(
                    "Product marked as having variants must have at least one attribute and one variant");

            foreach (var attr in product.Attributes)
            {
                attr.ProductId = product.Id;
            }

            foreach (var variant in product.Variants)
            {
                variant.ProductId = product.Id;
            }

            product.Price = product.Variants.Min(v => v.Price);

            var promotionalPrices = product.Variants
                .Where(v => v.PromotionalPrice.HasValue)
                .Select(v => v.PromotionalPrice.Value);

            product.PromotionalPrice = promotionalPrices.Any() ? promotionalPrices.Min() : null;

            product.Stock = product.Variants.Sum(v => v.Stock);
        }
        else
        {
            product.Attributes.Clear();
            product.Variants.Clear();
        }

        await _marketplaceProductRepository.AddAsync(product, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MarketplaceProductResponse
        {
            Success = true,
            Message = "Product created successfully",
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ProductType = product.ProductType,
            PromotionalPrice = product.PromotionalPrice,
            Stock = product.Stock
        };
    }
}