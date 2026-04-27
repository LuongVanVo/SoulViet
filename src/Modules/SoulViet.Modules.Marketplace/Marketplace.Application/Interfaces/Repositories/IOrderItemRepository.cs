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

    // Lấy tất cả các món hàng đã Delivered nhưng chưa đối soát (chưa được đánh dấu IsSettled = true) để chuẩn bị cho việc tạo PayoutBatch
    Task<List<OrderItem>> GetUnsettledItemsAsync(DateTime endDate, CancellationToken cancellationToken = default);

    // Cập nhật danh sách các món hàng đã được đối soát (đánh dấu IsSettled = true và gán PayoutBatchId) sau khi tạo PayoutBatch
    void UpdateRange(IEnumerable<OrderItem> orderItems);
}