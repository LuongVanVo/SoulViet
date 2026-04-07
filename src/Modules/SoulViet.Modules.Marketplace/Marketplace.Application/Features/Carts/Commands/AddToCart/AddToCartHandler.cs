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
        var product = await _marketplaceProductRepository.GetByIdAsync(request.MarketplaceProductId, cancellationToken);
        if (product == null || !product.IsActive)
            throw new NotFoundException("Marketplace product not found or is inactive.");

        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId, Items = new List<CartItem>() };
        }

        var existingItem = cart.GetCartItem(request.MarketplaceProductId, request.ItemMetadata);
        if (existingItem != null)
        {
            if (existingItem.Quantity + request.Quantity > product.Stock)
                throw new BadRequestException("Requested quantity exceeds available stock.");

            existingItem.Quantity += request.Quantity;
            existingItem.MarketplaceProduct = product;
        }
        else
        {
            if (request.Quantity > product.Stock)
                throw new BadRequestException("Requested quantity exceeds available stock.");

            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                MarketplaceProductId = request.MarketplaceProductId,
                Quantity = request.Quantity,
                ItemMetadata = request.ItemMetadata,
                MarketplaceProduct = product
            });
        }

        await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);

        return _mapper.Map<CartDto>(cart);
    }
}