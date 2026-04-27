namespace SoulViet.Modules.Marketplace.Marketplace.Application.Common.Events;

public class OrderPaymentSuccessEvent
{
    public Guid MasterOrderId { get; set; }
}