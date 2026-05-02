# Enum of EntityEnumConfigurationShare
namespace SoulViet.Shared.Domain.Enums
{
    public enum MediaType
    {
        Image = 1,
        Video = 2
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoulViet.Shared.Domain.Enums
{
    public enum NotificationTargetType
    {
        [Description("None")]
        None = 0,
        [Description("Post")]
        Post = 1,
        [Description("Comment")]
        Comment = 2,
        [Description("User")]
        User = 3,
        [Description("Follow")]
        Follow = 4
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoulViet.Shared.Domain.Enums
{
    public enum NotificationType
    {
        [Description("Thích bài viết")]
        Liked = 1,
        [Description("Bình luận bài viết")]
        Commented = 2,
        [Description("Theo dõi")]
        Followed = 3,
        [Description("Nhắc đến")]
        Mentioned = 4,
        [Description("Chia sẻ bài viết")]
        Shared = 5,
        [Description("Tài khoản đã được xác minh")]
        PartnerVerified = 6
    }
}

namespace SoulViet.Shared.Domain.Enums;

public enum PartnerType
{
    CoSoBanDia = 1,
    HuongDanVien = 2,
}

using System.ComponentModel;

namespace SoulViet.Shared.Domain.Enums
{
    public enum ShareType
    {
        [Description("Share lên Timeline")]
        Timeline = 1,

        [Description("Share qua tin nhắn")]
        Message = 2,

        [Description("Share ra bên ngoài")]
        External = 3
    }
}

using System.ComponentModel;

namespace SoulViet.Shared.Domain.Enums
{
    public enum VibeTag
    {
        [Description("Chữa lành & Yên bình")]
        ChuaLanhVaYenBinh = 1,
        [Description("Năng động & Phiêu lưu")]
        NangDongVaPhieuLuu = 2,
        [Description("Sang trọng & Đẳng cấp")]
        SangTrongVaDangCap = 3,

        [Description("Sáng tạo & Truyền cảm hứng")]
        SangTaoVaTruyenCamHung = 4,

        [Description("Trải nghiệm đa dạng")]
        TraiNghiemDaDang = 5,

        [Description("Đậm văn hóa & Bản địa")]
        DamVanHoaVaBanDia = 6
    }
}

# Entity of EntityEnumConfigurationShare
using SoulViet.Shared.Domain.Common;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Shared.Domain.Entities
{
    public class LocalPartnerProfile : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PartnerType PartnerType { get; set; }
        public bool IsAuthenticCertified { get; set; } = false;
        public string TaxId { get; set; } = string.Empty; // Mã số thuế
        public TourGuideDetail? TourGuideDetail { get; set; }
    }
}

namespace SoulViet.Shared.Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}

namespace SoulViet.Shared.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}

namespace SoulViet.Shared.Domain.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}

namespace SoulViet.Shared.Domain.Entities;

public class TourGuideDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LocalPartnerProfileId { get; set; }
    public LocalPartnerProfile LocalPartnerProfile { get; set; } = null!;

    // Thông tin chuyên môn
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal PricePerHour { get; set; }
    public double AverageRating { get; set; }

    public List<string> Languages { get; set; } = new();
    public List<string> Specialties { get; set; } = new();
    public List<string> CoverageProvinces { get; set; } = new();
}

using SoulViet.Shared.Domain.Common;

namespace SoulViet.Shared.Domain.Entities
{
    public class User : BaseAuditableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public int SoulCoinBalance { get; set; } = 0;

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsGoogleAccount { get; set; } = false;
        public string? VerficationToken { get; set; } = string.Empty;
        public DateTime? VerficationTokenExpiry { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        // Relationship
        public List<UserRole> UserRoles { get; set; } = new();
    }
}

using SoulViet.Shared.Domain.Common;

namespace SoulViet.Shared.Domain.Entities
{
    public class User : BaseAuditableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public int SoulCoinBalance { get; set; } = 0;

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsGoogleAccount { get; set; } = false;
        public string? VerficationToken { get; set; } = string.Empty;
        public DateTime? VerficationTokenExpiry { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        // Relationship
        public List<UserRole> UserRoles { get; set; } = new();
    }
}

namespace SoulViet.Shared.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; } 
        public Guid RoleId { get; set; }
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}

namespace SoulViet.Shared.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } 
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
    }
}

# Configuration of EntityEnumConfigurationShare
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class LocalPartnerProfileConfiguration : IEntityTypeConfiguration<LocalPartnerProfile>
    {
        public void Configure(EntityTypeBuilder<LocalPartnerProfile> builder)
        {
            builder.ToTable("LocalPartnerProfiles", "public");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId).IsUnique(); // Each user can have only one local partner profile

            builder.Property(x => x.BusinessName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(2000);
            builder.Property(x => x.TaxId).HasMaxLength(50);

            builder.Property(x => x.IsAuthenticCertified).HasDefaultValue(false);

            builder.Property(x => x.PartnerType).HasConversion<int>().IsRequired();

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<LocalPartnerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TourGuideDetail)
                .WithOne(t => t.LocalPartnerProfile)
                .HasForeignKey<TourGuideDetail>(t => t.LocalPartnerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", "public");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}

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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions", "public");

            // Composite primary key
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.HasOne<Role>()
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations;

public class TourGuideDetailConfiguration : IEntityTypeConfiguration<TourGuideDetail>
{
    public void Configure(EntityTypeBuilder<TourGuideDetail> builder)
    {
        builder.ToTable("TourGuideDetails", "public");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LicenseNumber).HasMaxLength(200);
        builder.Property(x => x.PricePerDay).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PricePerHour).HasColumnType("decimal(18,2)");

        builder.Property(x => x.Languages).HasColumnType("jsonb");
        builder.Property(x => x.Specialties).HasColumnType("jsonb");
        builder.Property(x => x.CoverageProvinces).HasColumnType("jsonb");
    }
}

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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles", "public");
            
            // Composite primary key
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Relationships 
            builder.HasOne(ur => ur.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.HasOne(ur => ur.Role) 
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions", "public");
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

# Base Auditable Entity of EntityEnumConfigurationShare
namespace SoulViet.Shared.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy  { get; set; }
    }
}