using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Configurations
{
    public class TouristAttractionConfiguration : IEntityTypeConfiguration<TouristAttraction>
    {
        public void Configure(EntityTypeBuilder<TouristAttraction> builder)
        {
            builder.ToTable("TouristAttractions", "soulmap");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.PartnerId);

            // Mối quan hệ với Category
            builder.Property(x => x.CategoryId).IsRequired();
            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 

            // THÊM MỚI: Mối quan hệ với Province
            builder.Property(x => x.ProvinceId).IsRequired();
            builder.HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình các trường chuỗi cơ bản
            builder.Property(x => x.PlaceId);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Type).HasMaxLength(100);
            builder.Property(x => x.Address);
            builder.Property(x => x.Description);
            builder.Property(x => x.OperationHours);

            // Location (PostGIS)
            builder.Property(x => x.Location)
                .HasColumnType("geometry(Point, 4326)")
                .IsRequired();
            builder.HasIndex(x => x.Location).HasMethod("GIST");

            builder.Property(x => x.RatingScore).HasDefaultValue(0);
            builder.Property(x => x.ReviewCount).HasDefaultValue(0);
            builder.Property(x => x.ReferencePrice).HasMaxLength(100);

            builder.Property(x => x.AllTypes).HasColumnType("jsonb");
            builder.Property(x => x.Activities).HasColumnType("jsonb");
            builder.Property(x => x.TopReviews).HasColumnType("jsonb");

            builder.OwnsOne(x => x.Media, media =>
            {
                media.ToJson("MediaInfo");
                media.Property(m => m.MainImage).HasMaxLength(500);
            });

            builder.Property(x => x.VibeTag).HasConversion<int>();
            builder.Property(x => x.BudgetTag).HasMaxLength(50);
            builder.Property(x => x.AiContext);
        }
    }
}