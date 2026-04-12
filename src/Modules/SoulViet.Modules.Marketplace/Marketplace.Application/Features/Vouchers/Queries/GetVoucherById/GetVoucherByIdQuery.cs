using System.Text.Json.Serialization;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVoucherById;

public class GetVoucherByIdQuery : IRequest<VoucherDto>
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid? PartnerId { get; set; }
    [JsonIgnore]
    public bool IsAdmin { get; set; }
}