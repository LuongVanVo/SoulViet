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