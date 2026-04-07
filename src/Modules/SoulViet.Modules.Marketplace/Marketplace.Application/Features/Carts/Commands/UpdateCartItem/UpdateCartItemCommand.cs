using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommand : IRequest<CartDto>
{
    public Guid UserId { get; set; }
    public Guid CartItemId { get; set; }
    public int NewQuantity { get; set; }
    public string ItemMetadata { get; set; } = string.Empty;
}