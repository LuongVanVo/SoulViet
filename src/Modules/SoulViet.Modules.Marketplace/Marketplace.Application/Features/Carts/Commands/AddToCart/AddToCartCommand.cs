using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.AddToCart;

public class AddToCartCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
    public Guid MarketplaceProductId { get; set; }
    public int Quantity { get; set; }
    public string? ItemMetadata { get; set; }
}