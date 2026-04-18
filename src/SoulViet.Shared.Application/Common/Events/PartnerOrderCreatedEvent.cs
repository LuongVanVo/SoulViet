namespace SoulViet.Shared.Application.Common.Events;

public class PartnerOrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid PartnerId { get; set; }
    public string PartnerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Language { get; set; } = "vi";
}