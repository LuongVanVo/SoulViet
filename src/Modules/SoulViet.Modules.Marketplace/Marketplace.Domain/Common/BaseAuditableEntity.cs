namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? LastModifiedBy  { get; set; }
    }
}