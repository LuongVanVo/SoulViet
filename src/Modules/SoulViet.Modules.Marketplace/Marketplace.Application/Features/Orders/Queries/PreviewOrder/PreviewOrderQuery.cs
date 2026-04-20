using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.PreviewOrder;

public class PreviewOrderQuery : IRequest<PreviewOrderResponse>
{
    public Guid UserId { get; set; }
    public List<Guid> SelectedCartItemIds { get; set; } = new();
    public string? PlatformVoucherCode { get; set; }
    public Dictionary<Guid, string> ShopVoucherCodes { get; set; } = new();
    public bool UseSoulCoin { get; set; }
    public decimal SoulCoinAmountToUse { get; set; }
}