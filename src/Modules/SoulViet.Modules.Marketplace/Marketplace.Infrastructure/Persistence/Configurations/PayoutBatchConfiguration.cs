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