using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

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

        // Fields about finance
        public decimal CommissionRate { get; set; } // % Hoa hồng lúc mua 
        public decimal PlatformFee { get; set; }
        public decimal PartnerEarnings { get; set; }

        public Order Order { get; set; } = null!;
    }
}