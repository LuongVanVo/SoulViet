using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

// Đại  diện cho 1 lần thanh toán
public class MasterOrder : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public decimal TotalItemsPrice { get; set; }
    public decimal TotalShippingFee { get; set; }

    public string? PlatformVoucherCode { get; set; }
    public decimal PlatformDiscountAmount { get; set; }
    public decimal GrandTotal { get; set; } // Tổng tiền toàn bộ giỏ

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Một lần thanh toán có thể có nhiều đơn hàng của các partner
    public ICollection<Order> VendorOrders { get; set; } = new List<Order>();
}