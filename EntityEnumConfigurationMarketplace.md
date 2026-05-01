# Entities of Marketplace Module
using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class Cart : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public CartItem? GetCartItem(Guid marketplaceProductId, string? metadata)
    {
        return Items.FirstOrDefault(x =>
            x.MarketplaceProductId == marketplaceProductId &&
            x.ItemMetadata == metadata);
    }

    public CartItem? GetCartItemById(Guid cartItemId)
    {
        return Items.FirstOrDefault(x => x.Id == cartItemId);
    }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class CartItem : BaseAuditableEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid MarketplaceProductId { get; set; }
    public MarketProduct MarketplaceProduct { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ItemMetadata { get; set; }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class MarketplaceCategory : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public ProductType CategoryType { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<MarketProduct> Products { get; set; } = new List<MarketProduct>();
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class MarketProduct : BaseAuditableEntity
    {
        public Guid PartnerId { get; set; }
        public Guid CategoryId { get; set; }
        public MarketplaceCategory MarketplaceCategory { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; } // Giá khuyến mãi
        public int Stock { get; set; }
        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }

        // Thông tin media liên quan đến sản phẩm
        public bool IsActive { get; set; } = true;
        public bool IsVerifiedByAdmin { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public ProductMediaInfo Media { get; set; } = new();
    }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

// Đại  diện cho 1 lần thanh toán
public class MasterOrder : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public decimal TotalItemsPrice { get; set; }
    public decimal TotalShippingFee { get; set; }

    public string? PlatformVoucherCode { get; set; }
    public decimal PlatformDiscountAmount { get; set; }
    public decimal GrandTotal { get; set; } // Tổng tiền toàn bộ giỏ

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal SoulCoinUsed { get; set; } // Số SoulCoin đã sử dụng trong đơn hàng này
    public decimal FinalPayableAmount { get; set; } // Số tiền cuối cùng khách phải trả sau khi trừ SoulCoin
    public string? SplitNote { get; set; } = string.Empty;

    // Một lần thanh toán có thể có nhiều đơn hàng của các partner
    public ICollection<Order> VendorOrders { get; set; } = new List<Order>();
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class Order : BaseAuditableEntity
    {
        public Guid MasterOrderId { get; set; } // Liên kết với lần thanh toán tổng
        public MasterOrder MasterOrder { get; set; } = null!;

        public Guid PartnerId { get; set; }
        public Guid UserId { get; set; }
        
        // Shipping info
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string OrderNotes { get; set; } = string.Empty; // Guest note

        public decimal ShippingFee { get; set; }
        public string? ShippingTrackingCode { get; set; } // Mã vận đơn
        public DateTime? ExpectedDeliveryOrServiceDate { get; set; } // Ngày dự kiến giao hàng hoặc cung cấp dịch vụ

        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }

        public string? ShopVoucherCode { get; set; }
        public decimal ShopDiscountAmount { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền của riêng Partner này

        // Status & Payment 
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public bool IsSettled { get; set; } = false;
        public Guid? SettlementId { get; set; } // Nằm trong kỳ đối soát nào

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class OrderItem : BaseAuditableEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;
        public string ProductImageSnapshot { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Lưu chuỗi JSON chứa thông tin ngày giờ book, size, color, v.v. để đảm bảo tính toàn vẹn của đơn hàng khi sản phẩm có thể thay đổi thông tin sau khi đặt hàng
        public string? ItemMetadata { get; set; }

        // Fields about finance
        public decimal CommissionRate { get; set; } // % Hoa hồng lúc mua 
        public decimal PlatformFee { get; set; } // Phí nền tảng
        public decimal PartnerEarnings { get; set; }
        public string? TicketCode { get; set; }  // Nếu là vé điện tử thì lưu mã vé ở đây
        public string? TicketQRUrl { get; set; } // Link QR code saved in cloud storage
        public ProductType ProductTypeSnapshot { get; set; }
        public bool IsTicketUsed { get; set; } = false;
        public DateTime? TicketUsedDate { get; set; }
        public bool IsSettled { get; set; } = false; // Đánh dấu đã được thanh toán cho đối tác
        public Guid? PayoutBatchId { get; set; } // Id của batch payout nếu đã được thanh toán (thuộc đợt thanh toán nào)
        public Order Order { get; set; } = null!;
    }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class PaymentTransaction : BaseAuditableEntity
{
    public Guid MasterOrderId { get; set; }
    public MasterOrder MasterOrder { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string TransactionRef { get; set; } = string.Empty;

    public string? GatewayTransactionNo { get; set; }
    public string? GatewayResponseCode { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? RawPayload { get; set; }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class PayoutBatch : BaseAuditableEntity
{
    public Guid PartnerId { get; set; }
    public DateTime PeriodStart { get; set; } // Thời gian bắt đầu của kỳ thanh toán (ví dụ: 01/09/2024)
    public DateTime PeriodEnd { get; set; } // Thời gian kết thúc của kỳ thanh toán (ví dụ: 30/09/2024)

    public decimal TotalSales { get; set; } // Tổng doanh thu của đối tác trong kỳ thanh toán
    public decimal TotalCommission { get; set; } // Tổng phí sàn thu
    public decimal NetPayout { get; set; } // Số tiền thực tế đối tác nhận được sau khi trừ phí sàn

    public SettlementStatus Status { get; set; }
    public string PartnerNameSnapshot { get; set; } = string.Empty; // Lưu tên đối tác tại thời điểm tạo batch để đảm bảo tính toàn vẹn thông tin khi đối tác có thể đổi tên sau đó
    public string? TransactionReference { get; set; }

    // Đợt đối soát này bao gồm những đơn hàng nào (có thể có nhiều đơn hàng)
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class ProductMediaInfo
    {
        public string MainImage { get; set; } = string.Empty;
        public List<string> LandImages { get; set; } = new();
        public string? VideoUrl { get; set; }
    }
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class SoulCoinTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public SoulCoinTransactionType Type { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class Voucher : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public VoucherScope Scope { get; set; } = VoucherScope.Platform;
    public Guid? PartnerId { get; set; } // If Scope = Shop, this is ID of the shop/partner

    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal MinOrderAmount { get; set; } = 0;

    public int UsageLimit { get; set; } = 1;
    public int UsedCount { get; set; } = 0;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

# Enums of Marketplace Module
namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

public enum DiscountType
{
    FixedAmount = 1,
    Percentage = 2,
    FreeShipping = 3
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Refunded = 6
    }
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums
{
    public enum PaymentMethod 
    {
        Cod = 1,
        VnPay = 2,
        SoulCoin = 3,
    }
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4,
        Canceled = 5
    }
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums
{
    public enum ProductType
    {
        PhysicalGoods = 1, // Hàng hóa vật lý cần giao hàng
        WorkshopTicket = 2, // Vé trải nghiệm dịch vụ / QR Code
    }
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums
{
    public enum SettlementStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
    }
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

public enum SoulCoinTransactionType
{
    Earned = 1, // Kiếm được Soul Coin (ví dụ: đăng bài được nhiều tương tác, tham gia sự kiện,....)
    Payment = 2, // Dùng Soul Coin để thanh toán đơn hàng
    Refund = 3, // Hoàn tiền Soul Coin khi đơn hàng bị hủy hoặc trả hàng
}

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

public enum VoucherScope
{
    Platform = 1,
    Shop = 2,
}

# Configuration of Marketplace Module
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique();

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa giỏ hàng thì xóa luôn các item trong đó
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CartId).IsRequired();
        builder.Property(x => x.MarketplaceProductId).IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        builder.Property(x => x.ItemMetadata).HasColumnType("jsonb"); // Lưu metadata dưới dạng JSONB

        // Relationships
        builder.HasOne(x => x.MarketplaceProduct)
            .WithMany()
            .HasForeignKey(x => x.MarketplaceProductId)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa sản phẩm nếu đang có trong giỏ hàng
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class MarketplaceCategoryConfiguration : IEntityTypeConfiguration<MarketplaceCategory>
{
    public void Configure(EntityTypeBuilder<MarketplaceCategory> builder)
    {
        builder.ToTable("Categories", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.CategoryType).HasConversion<int>().IsRequired();

        builder.Property(x => x.IsActive).HasDefaultValue(true);

        // Relationships
        builder.HasMany(x => x.Products)
            .WithOne(p => p.MarketplaceCategory)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations
{
    public class MarketProductConfiguration : IEntityTypeConfiguration<MarketProduct>
    {
        public void Configure(EntityTypeBuilder<MarketProduct> builder)
        {
            builder.ToTable("MarketProducts", "marketplace");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartnerId).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();

            builder.HasIndex(x => x.PartnerId);
            builder.HasIndex(x => x.CategoryId);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(4000);

            // Precision for Currency
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.PromotionalPrice).HasColumnType("decimal(18,2)");

            builder.Property(x => x.Stock).IsRequired().HasDefaultValue(0);

            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.IsVerifiedByAdmin).HasDefaultValue(false);

            // Enum 
            builder.Property(x => x.ProductType).HasConversion<int>().IsRequired();

            // Config ProductMediaInfo to JSON Object 
            builder.OwnsOne(x => x.Media, media =>
            {
                media.ToJson("MediaInfo");
                media.Property(m => m.MainImage).HasMaxLength(500);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class MasterOrderConfiguration : IEntityTypeConfiguration<MasterOrder>
{
    public void Configure(EntityTypeBuilder<MasterOrder> builder)
    {
        builder.ToTable("MasterOrders", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.TotalItemsPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalShippingFee).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.PlatformDiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.GrandTotal).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.PlatformVoucherCode).HasMaxLength(50);

        builder.Property(x => x.PaymentMethod).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();

        builder.Property(x => x.TransactionId).HasMaxLength(100);

        // Relationships
        builder.HasMany(x => x.VendorOrders)
            .WithOne(o => o.MasterOrder)
            .HasForeignKey(o => o.MasterOrderId)
            .OnDelete(DeleteBehavior.Cascade); // Hủy thanh toán thì hủy luôn đơn hàng con
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "marketplace");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.PartnerId).IsRequired();
            builder.Property(x => x.MasterOrderId).IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.PartnerId);
            builder.HasIndex(x => x.MasterOrderId);
            builder.HasIndex(x => x.SettlementId);

            builder.Property(x => x.ReceiverName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ReceiverPhone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(x => x.OrderNotes).HasMaxLength(1000);

            builder.Property(x => x.ShopVoucherCode).HasMaxLength(50);
            builder.Property(x => x.ShippingTrackingCode).HasMaxLength(150);
            builder.Property(x => x.CancellationReason).HasMaxLength(1000);

            // Precision for Currency
            builder.Property(x => x.ShippingFee).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.ShopDiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

            // Enums
            builder.Property(x => x.Status).HasConversion<int>();
            
            builder.Property(x => x.IsSettled).HasDefaultValue(false);

            // Relationships
            builder.HasMany(x => x.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "marketplace");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.TicketCode);

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();

            // Snapshot Name & Image (Nên luôn bắt cứng khi user mua, sp có đổi tên sau đó thì bill vẫn hiển thị đúng lúc mua)
            builder.Property(x => x.ProductNameSnapshot).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ProductImageSnapshot).HasMaxLength(500);

            builder.Property(x => x.Quantity).IsRequired();

            builder.Property(x => x.ItemMetadata).HasColumnType("jsonb"); // Lưu metadata dưới dạng JSONB

            // Tiền tệ và tính toán
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.CommissionRate).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(x => x.PlatformFee).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.PartnerEarnings).HasColumnType("decimal(18,2)").IsRequired();

            // Payout & Settlement
            builder.Property(x => x.PayoutBatchId).IsRequired(false);
            builder.Property(x => x.IsSettled).HasDefaultValue(false);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MasterOrderId).IsRequired();

        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.PaymentMethod).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();

        builder.Property(x => x.TransactionRef).IsRequired().HasMaxLength(150);
        builder.Property(x => x.GatewayTransactionNo).HasMaxLength(150);
        builder.Property(x => x.GatewayResponseCode).HasMaxLength(100);

        builder.Property(x => x.RawPayload).HasColumnType("jsonb");

        // Relationships
        builder.HasOne(x => x.MasterOrder)
            .WithMany()
            .HasForeignKey(x => x.MasterOrderId)
            .OnDelete(DeleteBehavior.Cascade); // Nếu xóa đơn hàng tổng thì xóa luôn giao dịch thanh toán liên quan
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class PayoutBatchConfiguration : IEntityTypeConfiguration<PayoutBatch>
{
    public void Configure(EntityTypeBuilder<PayoutBatch> builder)
    {
        builder.ToTable("PayoutBatches", "marketplace");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalSales).HasPrecision(18, 2);
        builder.Property(x => x.TotalCommission).HasPrecision(18, 2);
        builder.Property(x => x.NetPayout).HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.TransactionReference)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasMany(x => x.OrderItems)
            .WithOne() // OrderItem không nhất thiết phải có Navigation Property ngược lại nếu bạn không cần
            .HasForeignKey(x => x.PayoutBatchId)
            .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Batch nếu đang có Item dính vào
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class SoulCoinTransactionConfiguration : IEntityTypeConfiguration<SoulCoinTransaction>
{
    public void Configure(EntityTypeBuilder<SoulCoinTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("SoulCoinTransactions", "marketplace");

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceId).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasIndex(x => x.UserId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();

        // Enums
        builder.Property(x => x.Scope).HasConversion<int>().IsRequired();
        builder.Property(x => x.DiscountType).HasConversion<int>().IsRequired();

        builder.HasIndex(x => x.PartnerId);

        // Precision for Currency
        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.UsageLimit).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.UsedCount).IsRequired().HasDefaultValue(0);

        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}