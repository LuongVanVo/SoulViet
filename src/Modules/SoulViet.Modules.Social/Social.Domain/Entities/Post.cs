using SoulViet.Modules.Social.Social.Domain.Common;
using SoulViet.Modules.Social.Social.Domain.Enums;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class Post : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public List<Guid> TaggedProductIds { get; set; } = new();
        public VibeTag VibeTag { get; set; }

        public Guid? CheckinLocationId { get; set; } // Link to SoulMap
        public string? CheckinLocationName { get; set; }
        public Guid? OriginalPostId { get; set; } // Link to source post when shared to Timeline

        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public int SharesCount { get; set; } = 0;
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public double TrendingScore { get; set; } = 0;
        public string? AspectRatio { get; set; } // horizontal, vertical, square

        public List<PostComment> Comments { get; set; } = new();
        public List<PostLike> Likes { get; set; } = new();
        public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
        public bool IsDeleted { get; set; } = false;
    }
}