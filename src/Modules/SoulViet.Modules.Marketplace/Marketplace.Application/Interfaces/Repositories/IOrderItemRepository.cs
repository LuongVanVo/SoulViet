using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Lấy chi tiết vé kèm theo thông tin của shop (vendor order)
    Task<OrderItem?> GetTicketByIdWithOrderAsync(Guid id, CancellationToken cancellationToken = default);

    // Lấy tất cả các món hàng trong một VendorOrder để kiểm tra xem đã check-in hết chưa
    Task<List<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    // Cập nhật
    void Update(OrderItem orderItem);
}