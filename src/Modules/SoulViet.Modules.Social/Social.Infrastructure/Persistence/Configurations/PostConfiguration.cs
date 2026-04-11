using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts", "social");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Content).IsRequired();

            builder.Property(x => x.MediaUrls).HasColumnType("text[]");

            builder.Property(x => x.VibeTag).HasConversion<int>().IsRequired();

            builder.HasIndex(x => x.CheckinLocationId);

            builder.Property(x => x.LikesCount).HasDefaultValue(0);
            builder.Property(x => x.CommentsCount).HasDefaultValue(0);
            builder.Property(x => x.SharesCount).HasDefaultValue(0);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}