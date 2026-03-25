using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class LocalPartnerProfileConfiguration
    {
        public void Configure(EntityTypeBuilder<LocalPartnerProfile> builder)
        {
            builder.ToTable("LocalPartnerProfiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId).IsUnique(); // Each user can have only one local partner profile

            builder.Property(x => x.BusinessName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(2000);
            builder.Property(x => x.TaxId).HasMaxLength(50);

            builder.Property(x => x.IsAuthenticCertified).HasDefaultValue(false);

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<LocalPartnerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}