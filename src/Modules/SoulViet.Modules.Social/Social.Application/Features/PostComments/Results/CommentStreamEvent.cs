using System;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Results
{
    public class CommentStreamEvent
    {
        public string Type { get; set; } = string.Empty; // "CREATED", "UPDATED", "DELETED"
        public PostCommentResponse? Comment { get; set; }
        public int CommentsCount { get; set; }
        public Guid PostId { get; set; }

        public static CommentStreamEvent Created(PostCommentResponse comment, int count) => new()
        {
            Type = "CREATED",
            Comment = comment,
            CommentsCount = count,
            PostId = comment.PostId
        };

        public static CommentStreamEvent Updated(PostCommentResponse comment, int count) => new()
        {
            Type = "UPDATED",
            Comment = comment,
            CommentsCount = count,
            PostId = comment.PostId
        };

        public static CommentStreamEvent Deleted(Guid commentId, Guid postId, int count) => new()
        {
            Type = "DELETED",
            Comment = new PostCommentResponse { Id = commentId, PostId = postId },
            CommentsCount = count,
            PostId = postId
        };
    }
}
