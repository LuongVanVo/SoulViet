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
            builder.HasIndex(x => x.SettlementId);

            builder.Property(x => x.ReceiverName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ReceiverPhone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(x => x.OrderNotes).HasMaxLength(1000);

            // Precision for Currency
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