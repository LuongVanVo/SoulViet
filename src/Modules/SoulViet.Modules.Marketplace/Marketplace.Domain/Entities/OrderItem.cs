using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class OrderItem : BaseAuditableEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;
        public string ProductImageSnapshot { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Lưu chuỗi JSON chứa thông tin ngày giờ book, size, color, v.v. để đảm bảo tính toàn vẹn của đơn hàng khi sản phẩm có thể thay đổi thông tin sau khi đặt hàng
        public string? ItemMetadata { get; set; }

        // Fields about finance
        public decimal CommissionRate { get; set; } // % Hoa hồng lúc mua 
        public decimal PlatformFee { get; set; } // Phí nền tảng
        public decimal PartnerEarnings { get; set; }
        public string? TicketCode { get; set; }  // Nếu là vé điện tử thì lưu mã vé ở đây
        public string? TicketQRUrl { get; set; } // Link QR code saved in cloud storage
        public ProductType ProductTypeSnapshot { get; set; }
        public bool IsTicketUsed { get; set; } = false;
        public DateTime? TicketUsedDate { get; set; }
        public bool IsSettled { get; set; } = false; // Đánh dấu đã được thanh toán cho đối tác
        public Guid? PayoutBatchId { get; set; } // Id của batch payout nếu đã được thanh toán (thuộc đợt thanh toán nào)
        public Order Order { get; set; } = null!;
    }
}