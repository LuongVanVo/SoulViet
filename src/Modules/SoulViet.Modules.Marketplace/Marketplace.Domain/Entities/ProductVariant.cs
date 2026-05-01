using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class ProductVariant : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public MarketProduct Product { get; set; } = null!;

    public string Sku { get; set; } = string.Empty; // Mã sản phẩm / vé nội bộ của shop

    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; } // Giá khuyến mãi (nếu có)
    public int Stock { get; set; } // Số lượng tồn kho

    // Lưu trữ tổ hợp tùy chọnn
    // VD Vật lý: {"Color": "Red", "Size": "M"}
    // VD Tour: {"Loại vé": "Người lớn", "Ngày trải nghiệm": "2024-09-01"}
    public string AttributesJson { get; set; } = "{}";

    public string? ImageUrl { get; set; } // Ảnh đại diện cho biến thể này (nếu có)
    public bool IsActive { get; set; } = true;
}