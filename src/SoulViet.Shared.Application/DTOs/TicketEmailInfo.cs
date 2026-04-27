namespace SoulViet.Shared.Application.DTOs;

public class TicketEmailInfo
{
    public string ProductName { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string QrUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
}