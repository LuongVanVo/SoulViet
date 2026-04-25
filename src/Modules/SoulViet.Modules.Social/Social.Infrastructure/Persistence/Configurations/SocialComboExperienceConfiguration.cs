using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class SocialComboExperienceConfiguration : IEntityTypeConfiguration<SocialComboExperience>
    {
        public void Configure(EntityTypeBuilder<SocialComboExperience> builder)
        {
            builder.ToTable("SocialComboExperiences", "social");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.GuideId);
            builder.HasIndex(x => x.ServicePartnerId);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Property(x => x.PromotionalPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.MediaUrls).HasColumnType("text[]");

            builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.HasQueryFilter(x => !x.IsDeleted); 
        }
    }
}
