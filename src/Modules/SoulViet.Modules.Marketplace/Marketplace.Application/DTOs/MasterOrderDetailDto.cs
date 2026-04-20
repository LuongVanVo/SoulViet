namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class MasterOrderDetailDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public decimal TotalItemsPrice { get; set; }
    public decimal TotalShippingFee { get; set; }
    public decimal PlatformDiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ShippingAddress { get; set; }

    public List<VendorOrderDetailDto> VendorOrders { get; set; } = new();
}

public class VendorOrderDetailDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal ShopDiscountAmount { get; set; }
    public string? OrderNotes { get; set; }

    public List<OrderItemDetailDto> Items { get; set; } = new();
}

public class OrderItemDetailDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ItemMetadata { get; set; }
}