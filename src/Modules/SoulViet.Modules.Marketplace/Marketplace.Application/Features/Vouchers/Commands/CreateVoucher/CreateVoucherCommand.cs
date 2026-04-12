using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherCommand : IRequest<VoucherResponse>
{
    public string Code { get; set; } = string.Empty;

    public VoucherScope Scope { get; set; }
    public Guid? PartnerId { get; set; } // Id của đối tác nếu là voucher riêng của đối tác

    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; } // Chỉ áp dụng cho voucher giảm theo phần trăm
    public decimal MinOrderAmount { get; set; } // Giá trị đơn hàng tối thiểu để áp dụng voucher

    public int UsageLimit { get; set; } // Số lần voucher có thể được sử dụng
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}