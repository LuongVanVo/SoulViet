using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", "marketplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CartId).IsRequired();
        builder.Property(x => x.MarketplaceProductId).IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        builder.Property(x => x.ItemMetadata).HasColumnType("jsonb"); // Lưu metadata dưới dạng JSONB

        // Relationships
        builder.HasOne(x => x.MarketplaceProduct)
            .WithMany()
            .HasForeignKey(x => x.MarketplaceProductId)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa sản phẩm nếu đang có trong giỏ hàng
    }
}