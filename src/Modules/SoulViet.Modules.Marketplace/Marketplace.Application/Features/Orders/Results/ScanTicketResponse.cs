namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

public class ScanTicketResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime UsedAt { get; set; }
}