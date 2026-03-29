using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations;

public class TourGuideDetailConfiguration : IEntityTypeConfiguration<TourGuideDetail>
{
    public void Configure(EntityTypeBuilder<TourGuideDetail> builder)
    {
        builder.ToTable("TourGuideDetails", "public");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LicenseNumber).HasMaxLength(200);
        builder.Property(x => x.PricePerDay).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PricePerHour).HasColumnType("decimal(18,2)");

        builder.Property(x => x.Languages).HasColumnType("jsonb");
        builder.Property(x => x.Specialties).HasColumnType("jsonb");
        builder.Property(x => x.CoverageProvinces).HasColumnType("jsonb");
    }
}