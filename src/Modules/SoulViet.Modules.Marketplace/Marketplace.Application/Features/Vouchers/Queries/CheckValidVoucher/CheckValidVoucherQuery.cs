using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.CheckValidVoucher;

public class CheckValidVoucherQuery : IRequest<VoucherDto>
{
    public string Code { get; set; } = string.Empty;
    public Guid? PartnerId { get; set; } // Id của đối tác để kiểm tra voucher riêng của đối tác
    public decimal CurrentOrderAmount { get; set; }
}