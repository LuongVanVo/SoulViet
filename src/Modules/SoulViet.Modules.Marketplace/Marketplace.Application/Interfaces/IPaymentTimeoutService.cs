namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

public interface IPaymentTimeoutService
{
    Task ProcessTimeoutAsync(Guid masterOrderId, CancellationToken cancellationToken = default);
}