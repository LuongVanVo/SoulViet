namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class MyTicketDetailDto
{
    public Guid OrderItemId { get; set; }
    public Guid MasterOrderId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string? TicketQrUrl { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? TicketUsedDate { get; set; }
    public DateTime? ServiceDate { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public bool IsExpired { get; set; }
    public string? ItemMetadata { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}