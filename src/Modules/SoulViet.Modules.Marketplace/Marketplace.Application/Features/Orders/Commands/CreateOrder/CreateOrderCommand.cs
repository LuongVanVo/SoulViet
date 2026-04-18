using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public Guid UserId { get; set; }

    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string OrderNotes { get; set; } = string.Empty;

    public List<Guid> SelectedCartItemIds { get; set; } = new();
    public string? PlatformVoucherCode { get; set; }
    public Dictionary<Guid, string> ShopVoucherCodes { get; set; } = new();

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;
}