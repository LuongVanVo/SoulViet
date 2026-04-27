using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class PayoutBatch : BaseAuditableEntity
{
    public Guid PartnerId { get; set; }
    public DateTime PeriodStart { get; set; } // Thời gian bắt đầu của kỳ thanh toán (ví dụ: 01/09/2024)
    public DateTime PeriodEnd { get; set; } // Thời gian kết thúc của kỳ thanh toán (ví dụ: 30/09/2024)

    public decimal TotalSales { get; set; } // Tổng doanh thu của đối tác trong kỳ thanh toán
    public decimal TotalCommission { get; set; } // Tổng phí sàn thu
    public decimal NetPayout { get; set; } // Số tiền thực tế đối tác nhận được sau khi trừ phí sàn

    public SettlementStatus Status { get; set; }
    public string PartnerNameSnapshot { get; set; } = string.Empty; // Lưu tên đối tác tại thời điểm tạo batch để đảm bảo tính toàn vẹn thông tin khi đối tác có thể đổi tên sau đó
    public string? TransactionReference { get; set; }

    // Đợt đối soát này bao gồm những đơn hàng nào (có thể có nhiều đơn hàng)
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}