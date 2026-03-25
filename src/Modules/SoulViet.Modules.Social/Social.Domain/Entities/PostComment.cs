using SoulViet.Modules.Social.Social.Domain.Common;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class PostComment : BaseAuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Post Post { get; set; } = null!;
    }
}