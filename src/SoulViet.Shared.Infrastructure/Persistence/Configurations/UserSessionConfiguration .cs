using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId); // Index for faster lookups by UserId

            builder.Property(x => x.RefreshToken).IsRequired().HasMaxLength(500);
            builder.HasIndex(x => x.RefreshToken).IsUnique(); // Ensure refresh tokens are unique

            builder.Property(x => x.DeviceInfo).HasMaxLength(500);
            builder.Property(x => x.IpAddress).HasMaxLength(50);

            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.IsRevoked).HasDefaultValue(false);

            // Relationship
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}