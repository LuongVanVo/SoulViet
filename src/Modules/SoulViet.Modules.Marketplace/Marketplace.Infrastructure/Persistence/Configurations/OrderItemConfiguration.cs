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