using SoulViet.Modules.Social.Social.Domain.Common;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class Post : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> MediaUrls { get; set; } = new();

        public VibeTag VibeTag { get; set; }

        public Guid? CheckinLocationId { get; set; } // Link to SoulMap

        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;

        // Navigation 
        public List<PostComment> Comments { get; set; } = new();
        public List<PostLike> Likes { get; set; } = new();
    }
}