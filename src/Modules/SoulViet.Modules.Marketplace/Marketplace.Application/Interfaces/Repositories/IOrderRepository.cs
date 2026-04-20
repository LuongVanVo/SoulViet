using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    void Update(Order order);
    Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(List<Order> Items, int TotalCount)> GetOrdersWithPaginationAsync(
        Guid? partnerId,
        OrderStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<(List<Order> Items, int TotalCount)> GetShopOrdersWithPaginationAsync(
        Guid partnerId,
        int pageNumber,
        int pageSize,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        PaymentMethod? paymentMethod,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    );

    Task <Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}