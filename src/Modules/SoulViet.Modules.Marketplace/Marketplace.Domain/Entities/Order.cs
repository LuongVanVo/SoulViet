using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class Order : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        
        // Shipping info
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string OrderNotes { get; set; } = string.Empty; // Guest note

        public decimal TotalAmount { get; set; }

        // Status & Payment 
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; } // Thời điểm tiền thực sự vào tài khoản

        public bool IsSettled { get; set; } = false;
        public Guid? SettlementId { get; set; } // Nằm trong kỳ đối soát nào
    }
}