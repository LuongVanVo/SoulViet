using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Results
{
    public class PostLikeResult
    {
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
        public Guid PostId { get; set; }

        public PostLikeResult(bool isLiked, int totalLikes, Guid postId)
        {
            IsLiked = isLiked;
            TotalLikes = totalLikes;
            PostId = postId;
        }
    }
}
