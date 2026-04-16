using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid MasterOrderId { get; set; }
    public Guid PartnerId { get; set; }

    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string OrderNotes { get; set; } = string.Empty;

    public decimal ShippingFee { get; set; }
    public string? ShopVoucherCode { get; set; }
    public decimal? ShopDiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = new();
}