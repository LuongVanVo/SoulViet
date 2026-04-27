namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;

public class PayoutBatchAdminResponse
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal NetPayout { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}