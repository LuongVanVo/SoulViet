using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "social");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RecipientUserId).IsRequired();
            builder.Property(x => x.ActorUserId).IsRequired();
            builder.Property(x => x.Type).HasConversion<int>().IsRequired();
            builder.Property(x => x.TargetType).HasConversion<int>().IsRequired();
            builder.Property(x => x.TargetId);

            builder.Property(x => x.IsRead).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.ReadAt);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.LastModifiedBy).HasMaxLength(100);

            builder.HasIndex(x => x.RecipientUserId);
            builder.HasIndex(x => x.ActorUserId);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.TargetType);
            builder.HasIndex(x => new { x.RecipientUserId, x.IsRead, x.CreatedAt });
        }
    }
}
