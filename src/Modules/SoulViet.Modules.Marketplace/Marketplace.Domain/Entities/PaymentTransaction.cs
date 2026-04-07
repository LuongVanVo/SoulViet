using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class PaymentTransaction : BaseAuditableEntity
{
    public Guid MasterOrderId { get; set; }
    public MasterOrder MasterOrder { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string TransactionRef { get; set; } = string.Empty;

    public string? GatewayTransactionNo { get; set; }
    public string? GatewayResponseCode { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? RawPayload { get; set; }
}