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
        bool needsRedisUpdate = false;

        foreach (var itemDto in cartDto.Items.ToList())
        {
            var domainItem = cart.Items.First(i => i.Id == itemDto.Id);

            if (productDict.TryGetValue(itemDto.MarketplaceProductId, out var latestProduct) && latestProduct.IsActive && !latestProduct.IsDeleted)
            {
                int currentStock = latestProduct.Stock;
                decimal currentPrice = latestProduct.Price;
                decimal? currentPromoPrice = latestProduct.PromotionalPrice;
                string currentImage = latestProduct.Media.MainImage;

                if (domainItem.VariantId.HasValue)
                {
                    var variant = latestProduct.Variants.FirstOrDefault(v => v.Id == domainItem.VariantId.Value);

                    if (variant != null && variant.IsActive)
                    {
                        currentStock = variant.Stock;
                        currentPrice = variant.Price;
                        currentPromoPrice = variant.PromotionalPrice;
                        currentImage = !string.IsNullOrEmpty(variant.ImageUrl) ? variant.ImageUrl : latestProduct.Media.MainImage;

                        // Gán AttributesJson để UI hiển thị
                        itemDto.VariantAttributesJson = variant.AttributesJson;
                    }
                    else
                    {
                        // Biến thể đã bị xóa/ẩn -> Xóa khỏi giỏ
                        cartDto.Items.Remove(itemDto);
                        cart.Items.Remove(domainItem);
                        needsRedisUpdate = true;
                        continue;
                    }
                }

                // Cập nhật thông tin hiển thị
                itemDto.ProductName = latestProduct.Name;
                itemDto.MainImage = currentImage;
                itemDto.Price = currentPrice;
                itemDto.PromotionalPrice = currentPromoPrice;
                itemDto.Stock = currentStock;

                // Xử lý tồn kho
                if (itemDto.Quantity > currentStock)
                {
                    if (currentStock <= 0)
                    {
                        cartDto.Items.Remove(itemDto);
                        cart.Items.Remove(domainItem);
                    }
                    else
                    {
                        itemDto.Quantity = currentStock;
                        domainItem.Quantity = currentStock;
                    }
                    needsRedisUpdate = true;
                }
            }
            else
            {
                // Sản phẩm gốc đã bị xóa/ẩn
                cartDto.Items.Remove(itemDto);
                cart.Items.Remove(domainItem);
                needsRedisUpdate = true;
            }
        }

        if (needsRedisUpdate)
        {
            await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);
        }

        return cartDto;
    }
}