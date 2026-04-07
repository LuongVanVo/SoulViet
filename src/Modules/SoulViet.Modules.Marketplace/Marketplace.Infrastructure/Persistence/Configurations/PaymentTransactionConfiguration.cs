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