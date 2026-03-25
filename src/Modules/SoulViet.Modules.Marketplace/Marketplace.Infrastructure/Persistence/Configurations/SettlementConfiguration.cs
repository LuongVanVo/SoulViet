using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations
{
    public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
    {
        public void Configure(EntityTypeBuilder<Settlement> builder)
        {
            builder.ToTable("Settlements", "marketplace");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PartnerId).IsRequired();
            builder.HasIndex(x => x.PartnerId);

            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            
            builder.Property(x => x.SettlementStatus).HasConversion<int>().IsRequired();
            builder.Property(x => x.SettelmentPeriod).IsRequired().HasMaxLength(50); // e.g: "Tháng 09/2023"
            
            builder.Property(x => x.BankTransferReference).HasMaxLength(150);
            
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}