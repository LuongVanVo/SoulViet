using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "public");

            // Primary key
            builder.HasKey(x => x.Id);

            // Property
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.AvatarUrl).IsRequired().HasMaxLength(1000);

            builder.Property(x => x.SoulCoinBalance).HasDefaultValue(0);

            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.Gender).HasMaxLength(50);

            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.IsEmailConfirmed).HasDefaultValue(false);
            builder.Property(x => x.IsGoogleAccount).HasDefaultValue(false);

            builder.Property(x => x.ConcurrencyStamp).IsConcurrencyToken();

            builder.Property(x => x.CreatedAt).IsRequired();

            // Relationship
            builder.HasMany(x => x.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}