using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class Voucher : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public VoucherScope Scope { get; set; } = VoucherScope.Platform;
    public Guid? PartnerId { get; set; } // If Scope = Shop, this is ID of the shop/partner

    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal MinOrderAmount { get; set; } = 0;

    public int UsageLimit { get; set; } = 1;
    public int UsedCount { get; set; } = 0;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}