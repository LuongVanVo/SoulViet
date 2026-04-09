// GetMyCartHandler.cs
using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Queries.GetMyCart;

public class GetCartQueryHandler : IRequestHandler<GetMyCartQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMarketplaceProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCartQueryHandler(
        ICartRepository cartRepository,
        IMarketplaceProductRepository productRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart == null || !cart.Items.Any())
            return new CartDto { UserId = request.UserId };

        var productIds = cart.Items.Select(i => i.MarketplaceProductId).Distinct().ToList();
        var currentProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productDict = currentProducts.ToDictionary(p => p.Id);

        var cartDto = _mapper.Map<CartDto>(cart);

        bool needsRedisUpdate = true;

        foreach (var itemDto in cartDto.Items.ToList())
        {
            if (productDict.TryGetValue(itemDto.MarketplaceProductId, out var latestProduct))
            {
                itemDto.ProductName = latestProduct.Name;
                itemDto.MainImage = latestProduct.Media.MainImage;
                itemDto.Price = latestProduct.Price;
                itemDto.PromotionalPrice = latestProduct.PromotionalPrice;
                itemDto.Stock = latestProduct.Stock;

                if (itemDto.Quantity > latestProduct.Stock)
                {
                    itemDto.Quantity = latestProduct.Stock;

                    var domainItem = cart.Items.First(i => i.Id == itemDto.Id);
                    domainItem.Quantity = itemDto.Quantity;
                }
            }
            else
            {
                cartDto.Items.Remove(itemDto);

                var domainItem = cart.Items.First(i => i.Id == itemDto.Id);
                cart.Items.Remove(domainItem);
            }
        }

        if (needsRedisUpdate)
        {
            await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);
        }

        return cartDto;
    }
}