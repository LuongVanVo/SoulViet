using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class Settlement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PartnerId { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền cần chuyển cho Partner đợt này
        public SettlementStatus SettlementStatus { get; set; } = SettlementStatus.Pending;
        public string SettelmentPeriod { get; set; } = string.Empty;
        public string? BankTransferReference { get; set; } // Mã giao dịch ngân hàng khi kế toán chuyển tiền cho Partner

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? TransferredAt { get; set; } 
    }
}