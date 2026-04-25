using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
    {
        public void Configure(EntityTypeBuilder<PostMedia> builder)
        {
            builder.ToTable("PostMedia", "social");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PostId).IsRequired();

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(2048); 

            builder.Property(x => x.ObjectKey)
                .IsRequired()
                .HasMaxLength(512); 
            builder.Property(x => x.MediaType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Width);
            builder.Property(x => x.Height);
            builder.Property(x => x.FileSizeBytes);

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.PostId);

            builder.HasIndex(x => new { x.PostId, x.SortOrder });

            builder.HasOne<Post>()
                .WithMany(p => p.Media)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
