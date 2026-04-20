namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class ShopOrderDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty; // Trạng thái giao hàng
    public decimal TotalAmount { get; set; }

    // Lấy chéo từ MasterOrder cho Shop yên tâm là khách đã trả tiền
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    public int TotalItems { get; set; } // Tổng số lượng sản phẩm khách mua
}