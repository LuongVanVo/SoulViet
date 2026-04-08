using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class Order : BaseAuditableEntity
    {
        public Guid MasterOrderId { get; set; } // Liên kết với lần thanh toán tổng
        public MasterOrder MasterOrder { get; set; } = null!;

        public Guid PartnerId { get; set; }
        public Guid UserId { get; set; }
        
        // Shipping info
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string OrderNotes { get; set; } = string.Empty; // Guest note

        public decimal ShippingFee { get; set; }
        public string? ShippingTrackingCode { get; set; } // Mã vận đơn
        public DateTime? ExpectedDeliveryOrServiceDate { get; set; } // Ngày dự kiến giao hàng hoặc cung cấp dịch vụ

        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }

        public string? ShopVoucherCode { get; set; }
        public decimal ShopDiscountAmount { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền của riêng Partner này

        // Status & Payment 
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public bool IsSettled { get; set; } = false;
        public Guid? SettlementId { get; set; } // Nằm trong kỳ đối soát nào

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}