namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class OrderHistoryItemDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public int TotalShops { get; set; }
    public List<string> ShopNames { get; set; } = new();
}