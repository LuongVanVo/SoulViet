using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Exceptions;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMarketplaceProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateCartItemHandler(
        ICartRepository cartRepository,
        IMarketplaceProductRepository productRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart == null)
            throw new NotFoundException("Cart not found.");

        var item = cart.GetCartItemById(request.CartItemId);
        if (item == null)
            throw new NotFoundException("Cart item not found in cart.");

        var product = await _productRepository.GetByIdWithDetailsAsync(item.MarketplaceProductId, cancellationToken);
        if (product == null || !product.IsActive)
            throw new NotFoundException("Marketplace product is no longer available.");

        int availableStock = product.Stock;
        if (item.VariantId.HasValue)
        {
            var variant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId.Value);
            if (variant == null || !variant.IsActive)
                throw new NotFoundException("Product variant is no longer available.");
            availableStock = variant.Stock;
        }

        if (request.NewQuantity > availableStock)
            throw new BadRequestException($"Sorry, the product only has {product.Stock} items left in stock.");

        item.Quantity = request.NewQuantity;

        await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);

        var cartDto = _mapper.Map<CartDto>(cart);

        var updatedItemDto = cartDto.Items.FirstOrDefault(i => i.Id == request.CartItemId);

        if (updatedItemDto != null)
        {
            updatedItemDto.ProductName = product.Name;
            updatedItemDto.MainImage = product.Media.MainImage;
            updatedItemDto.Price = product.Price;
            updatedItemDto.PromotionalPrice = product.PromotionalPrice;
            updatedItemDto.Stock = product.Stock;
            updatedItemDto.PartnerId = product.PartnerId;
        }

        return cartDto;
    }
}
