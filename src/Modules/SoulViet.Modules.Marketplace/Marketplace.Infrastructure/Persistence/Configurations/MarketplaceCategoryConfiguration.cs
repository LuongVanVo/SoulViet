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