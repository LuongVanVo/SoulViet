using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

public interface IBillSplitClient
{
    // Cập nhật lại toàn bộ phòng (khi có người vào/ra, đổi tiền)
    Task ReceiveRoomUpdate(SplitRoomSession room);

    // Gửi thông báo lỗi lên client
    Task ReceiveError(string message);

    // Thông báo toast nhỏ (ví dụ: "Bạn đã rời phòng", "Thanh toán thành công")
    Task ReceiveNotification(string message);

    // Bắn riêng link thanh toán cho từng thành viên
    Task ReceivePaymentUrl(string paymentUrl);

    // Thông báo cho cả phòng biết ai đó vừa thanh toán thành công
    Task ReceiveMemberPaidSuccess(Guid userId);
}