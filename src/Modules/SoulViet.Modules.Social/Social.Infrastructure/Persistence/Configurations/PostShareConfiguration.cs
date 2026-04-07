using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class PostShareConfiguration : IEntityTypeConfiguration<PostShare>
    {
        public void Configure(EntityTypeBuilder<PostShare> builder)
        {
            builder.ToTable("PostShares", "social");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Caption).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.LastModifiedBy).HasMaxLength(100);

            builder.HasIndex(x => x.PostId);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.PostId, x.UserId, x.CreatedAt });

            builder.HasOne(x => x.Post)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
