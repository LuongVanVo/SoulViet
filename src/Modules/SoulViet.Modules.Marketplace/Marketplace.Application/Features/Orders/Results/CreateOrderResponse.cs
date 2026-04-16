namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

public class CreateOrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid MasterOrderId { get; set; }
    public decimal GrandTotal { get; set; }
}