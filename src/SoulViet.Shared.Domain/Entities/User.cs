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