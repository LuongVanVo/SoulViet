using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class VoucherDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public VoucherScope Scope { get; set; }
    public Guid? PartnerId { get; set; } // Id của đối tác nếu là voucher riêng của đối tác
    public string? PartnerName { get; set; }

    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; } // Chỉ áp dụng cho voucher giảm theo phần trăm
    public decimal MinOrderAmount { get; set; } // Giá trị đơn hàng tối thiểu để áp dụng voucher

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; } // Số lần voucher có thể được sử dụng
    public int UsedCount { get; set; } // Số lần voucher đã được sử dụng
    public int RemainingCount => UsageLimit - UsedCount; // Số lần voucher còn lại có thể sử dụng
    public bool IsActive { get; set; }
}