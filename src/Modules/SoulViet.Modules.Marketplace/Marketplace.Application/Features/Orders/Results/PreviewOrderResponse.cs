namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

public class PreviewOrderResponse
{
    public List<PreviewVendorOrder> VendorOrders { get; set; } = new();

    public decimal TotalItemsPrice { get; set; }
    public decimal TotalShippingFee { get; set; }

    public string? PlatformVoucherCode { get; set; }
    public decimal PlatformDiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }
}

public class PreviewVendorOrder
{
    public Guid PartnerId { get; set; }

    public List<PreviewOrderItem> Items { get; set; } = new();

    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }

    public string? ShopVoucherCode { get; set; }
    public decimal ShopDiscountAmount { get; set; }

    public decimal TotalAmount { get; set; } // = SubTotal + ShippingFee - ShopDiscountAmount
}

public class PreviewOrderItem
{
    public Guid CartItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}