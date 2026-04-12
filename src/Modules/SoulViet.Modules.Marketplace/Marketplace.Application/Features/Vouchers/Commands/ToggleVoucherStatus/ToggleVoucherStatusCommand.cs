using System.Text.Json.Serialization;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.ToggleVoucherStatus;

public class ToggleVoucherStatusCommand : IRequest<VoucherResponse>
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid? PartnerId { get; set; }
    [JsonIgnore]
    public bool IsAdmin { get; set; }
}