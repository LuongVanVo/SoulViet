namespace SoulViet.Shared.Application.Common.Events;

public class UserOrderCreatedEvent
{
    public Guid MasterOrderId { get; set; }
    public Guid UserId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Language { get; set; } = "vi";
}