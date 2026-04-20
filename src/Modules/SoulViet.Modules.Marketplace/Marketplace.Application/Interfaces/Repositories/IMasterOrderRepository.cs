using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IMasterOrderRepository
{
    Task AddAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default);
    Task<MasterOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MasterOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    // Lịch sử mua hàng của Tourist
    Task<(List<MasterOrder> Items, int TotalCount)> GetByUserIdWithPaginationAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        PaymentStatus? paymentStatus,
        PaymentMethod? paymentMethod,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task<(List<MasterOrder> Items, int TotalCount)> GetMasterOrdersWithPaginationForAdminAsync(
        int pageNumber,
        int pageSize,
        PaymentStatus? paymentStatus,
        PaymentMethod? paymentMethod,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    );
}