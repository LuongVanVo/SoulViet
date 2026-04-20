namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

public enum SoulCoinTransactionType
{
    Earned = 1, // Kiếm được Soul Coin (ví dụ: đăng bài được nhiều tương tác, tham gia sự kiện,....)
    Payment = 2, // Dùng Soul Coin để thanh toán đơn hàng
    Refund = 3, // Hoàn tiền Soul Coin khi đơn hàng bị hủy hoặc trả hàng
}