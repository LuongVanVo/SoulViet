namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class AdminMasterOrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal GrandTotal { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public int VendorOrderCount { get; set; }
}