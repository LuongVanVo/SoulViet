using SoulViet.Shared.Domain.Common;

namespace SoulViet.Shared.Domain.Entities
{
    public class User : BaseAuditableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public int SoulCoinBalance { get; set; } = 0;

        // Relationship
        public List<UserRole> UserRoles { get; set; } = new();
    }
}