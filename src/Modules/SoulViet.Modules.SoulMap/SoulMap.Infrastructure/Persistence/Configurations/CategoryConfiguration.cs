using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", "soulmap");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.HasIndex(x => x.Name).IsUnique();
            
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            builder.HasIndex(x => x.Slug).IsUnique();

            builder.Property(x => x.IconUrl).HasMaxLength(500);
        }
    }
}