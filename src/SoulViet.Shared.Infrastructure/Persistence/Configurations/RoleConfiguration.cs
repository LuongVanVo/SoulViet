using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "public");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();

            // Relationship with RolePermission
            builder.HasMany(x => x.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            builder.HasData(
                new Role { Id = Guid.Parse("d9224bba-bd7d-4ee5-b958-9ae075e29784"), Name = "Tourist" },
                new Role { Id = Guid.Parse("a5ce151e-760f-44bb-a405-da38021917fc"), Name = "LocalPartner" },
                new Role { Id = Guid.Parse("d462852e-f2be-4eaa-88a5-cb0088db17f8"), Name = "Admin" }
            );
        }
    }
}