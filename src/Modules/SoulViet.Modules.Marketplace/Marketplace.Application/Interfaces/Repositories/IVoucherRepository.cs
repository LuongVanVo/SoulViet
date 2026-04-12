using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IVoucherRepository
{
    Task<Voucher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Voucher?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default);
    void Update(Voucher voucher);

    Task<Voucher?> GetValidVoucherAsync(string code, Guid? partnerId, CancellationToken cancellationToken = default);

    Task<List<Voucher>> GetAvailableVouchersAsync(Guid? partnerId, decimal currentOrderAmount, CancellationToken cancellationToken = default);

    Task<(List<Voucher> Items, int TotalCount)> GetVouchersWithPaginationAsync(
        Guid? partnerId,
        int pageNumber,
        int pageSize,
        string? searchTerm,
        bool? isActive,
        CancellationToken cancellationToken = default
    );
}