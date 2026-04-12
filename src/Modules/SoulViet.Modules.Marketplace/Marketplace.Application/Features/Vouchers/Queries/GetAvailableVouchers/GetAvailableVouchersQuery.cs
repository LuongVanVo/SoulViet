using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetAvailableVouchers;

public class GetAvailableVouchersQuery : IRequest<List<VoucherDto>>
{
    public Guid? PartnerId { get; set; }
    public decimal CurrentOrderAmount { get; set; }
}