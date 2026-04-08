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