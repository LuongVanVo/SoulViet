namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;

public class PayoutBatchDetailResponse
{
    public Guid BatchId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalNetPayout { get; set; }

    public List<SettledItemDetail> Items { get; set; } = new();
}

public class SettledItemDetail
{
    public Guid OrderItemId { get; set; }
    public Guid CustomerId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PartnerEarnings { get; set; }
    public DateTime OrderedAt { get; set; }
}