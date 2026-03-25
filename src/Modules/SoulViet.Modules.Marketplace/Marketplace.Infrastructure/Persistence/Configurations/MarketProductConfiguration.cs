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
            builder.HasIndex(x => x.PartnerId);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(4000);

            // Precision for Currency
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(x => x.Stock).IsRequired().HasDefaultValue(0);

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