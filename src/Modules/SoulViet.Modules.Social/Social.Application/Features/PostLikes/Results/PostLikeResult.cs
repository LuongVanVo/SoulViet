using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Results
{
    public class PostLikeResult
    {
        public bool IsLiked { get; set; }
        public int LikesCount { get; set; }
        public Guid PostId { get; set; }
        public Guid? ActorUserId { get; set; }

        public PostLikeResult(bool isLiked, int likesCount, Guid postId, Guid? actorUserId = null)
        {
            IsLiked = isLiked;
            LikesCount = likesCount;
            PostId = postId;
            ActorUserId = actorUserId;
        }
    }
}
