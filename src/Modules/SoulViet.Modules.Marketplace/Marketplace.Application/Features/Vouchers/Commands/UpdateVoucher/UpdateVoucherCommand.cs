using MediatR;
using System.Text.Json.Serialization;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.UpdateVoucher;

public class UpdateVoucherCommand : IRequest<VoucherResponse>
{
    public Guid Id { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }

    [JsonIgnore]
    public Guid? PartnerId { get; set; }
    [JsonIgnore]
    public bool IsAdmin { get; set; }
}