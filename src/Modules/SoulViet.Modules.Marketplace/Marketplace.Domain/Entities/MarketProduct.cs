using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class MarketProduct : BaseAuditableEntity
    {
        public Guid PartnerId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; } // Giá khuyến mãi
        public int Stock { get; set; }
        public ProductType ProductType { get; set; }

        // Thông tin media liên quan đến sản phẩm
        public bool IsActive { get; set; } = true;
        public bool IsVerifiedByAdmin { get; set; } = false;
        public ProductMediaInfo Media { get; set; } = new();
    }
}