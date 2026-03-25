using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Configurations
{
    public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
    {
        public void Configure(EntityTypeBuilder<Accommodation> builder)
        {
            builder.ToTable("Accommodations", "soulmap");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.PartnerId);

            builder.Property(x => x.ProvinceId).IsRequired();
            builder.HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => x.ProvinceId);

            // Các trường cơ bản
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Type).HasConversion<int>().IsRequired();
            builder.Property(x => x.Address).IsRequired();

            // Location
            builder.Property(x => x.Location)
                .HasColumnType("geometry(Point, 4326)")
                .IsRequired();
            builder.HasIndex(x => x.Location).HasMethod("GIST");

            // Giá cả & Điểm số
            builder.Property(x => x.PriceValue).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.PriceSegment).IsRequired(false);
            builder.Property(x => x.RatingScore).HasDefaultValue(0);
            builder.Property(x => x.ReviewCount).HasDefaultValue(0);

            // THÊM MỚI: Cấu hình cho các trường đánh giá
            builder.Property(x => x.StarRating).IsRequired(false);
            builder.Property(x => x.ReviewText);
            builder.Property(x => x.Highlights);
            builder.Property(x => x.BookingUrl);
            builder.Property(x => x.AiContext);
            builder.Property(x => x.VibeTag).HasConversion<int>();

            // JSONB
            builder.Property(x => x.FacilitiesJson).HasColumnType("jsonb");

            builder.OwnsOne(x => x.Media, media =>
            {
                media.ToJson("MediaInfo");
                media.Property(m => m.MainImage).HasMaxLength(500);
            });
        }
    }
}