using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Configurations
{
    public class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder.ToTable("UserFollowers", "social");

            builder.HasKey(x => new { x.FollowerId, x.FollowingId });

            builder.HasIndex(x => x.FollowerId);
            builder.HasIndex(x => x.FollowingId);

            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}