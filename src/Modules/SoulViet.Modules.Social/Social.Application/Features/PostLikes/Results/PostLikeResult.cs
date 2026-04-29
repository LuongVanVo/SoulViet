using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Results
{
    public class PostLikeResult
    {
        public bool IsLiked { get; set; }
        public int LikesCount { get; set; }
        public Guid PostId { get; set; }

        public PostLikeResult(bool isLiked, int likesCount, Guid postId)
        {
            IsLiked = isLiked;
            LikesCount = likesCount;
            PostId = postId;
        }
    }
}
