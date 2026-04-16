using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class MasterOrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public decimal TotalItemsPrice { get; set; }
    public decimal TotalShippingFee { get; set; }

    public string? PlatformVoucherCode { get; set; }
    public decimal PlatformDiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<OrderDto> VendorOrders { get; set; } = new();
}