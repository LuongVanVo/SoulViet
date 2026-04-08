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