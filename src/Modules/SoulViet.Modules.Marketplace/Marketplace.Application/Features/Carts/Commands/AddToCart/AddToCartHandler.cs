using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.AddToCart;

public class AddToCartHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public AddToCartHandler(
        ICartRepository cartRepository,
        IMarketplaceProductRepository marketplaceProductRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _marketplaceProductRepository = marketplaceProductRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _marketplaceProductRepository.GetByIdWithDetailsAsync(request.MarketplaceProductId, cancellationToken);
        if (product == null || !product.IsActive)
            throw new NotFoundException("Marketplace product not found or is inactive.");

        ProductVariant? variant = null;
        int availableStock = product.Stock;

        if (product.HasVariants)
        {
            if (!request.VariantId.HasValue)
                throw new BadRequestException("This product has many options, please select one of the options.");

            variant = product.Variants.FirstOrDefault(v => v.Id == request.VariantId.Value);
            if (variant == null || !variant.IsActive)
                throw new NotFoundException("Selected variant not found or is inactive.");

            availableStock = variant.Stock;
        }
        else if (request.VariantId.HasValue)
            throw new BadRequestException("This product does not have variants, variant selection is not required.");

        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId, Items = new List<CartItem>() };
        }

        var existingItem = cart.GetCartItem(request.MarketplaceProductId, request.VariantId, request.ItemMetadata);
        if (existingItem != null)
        {
            if (existingItem.Quantity + request.Quantity > availableStock)
                throw new BadRequestException("Requested quantity exceeds available stock.");

            existingItem.Quantity += request.Quantity;
            existingItem.MarketplaceProduct = product;
            existingItem.Variant = variant;
        }
        else
        {
            if (request.Quantity > availableStock)
                throw new BadRequestException("Requested quantity exceeds available stock.");

            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                MarketplaceProductId = request.MarketplaceProductId,
                VariantId = request.VariantId,
                Variant = variant,
                Quantity = request.Quantity,
                ItemMetadata = request.ItemMetadata,
                MarketplaceProduct = product
            });
        }

        await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);

        var cartDto = _mapper.Map<CartDto>(cart);

        if (request.VariantId.HasValue && variant != null)
        {
            var updatedItemDto = cartDto.Items.FirstOrDefault(i =>
                i.MarketplaceProductId == request.MarketplaceProductId &&
                i.VariantId == request.VariantId);

            if (updatedItemDto != null)
            {
                updatedItemDto.VariantAttributesJson = variant.AttributesJson;
                updatedItemDto.MainImage = !string.IsNullOrEmpty(variant.ImageUrl) ? variant.ImageUrl : product.Media.MainImage;
                updatedItemDto.Price = variant.Price;
                updatedItemDto.PromotionalPrice = variant.PromotionalPrice;
                updatedItemDto.Stock = variant.Stock;
            }
        }

        return cartDto;
    }
}