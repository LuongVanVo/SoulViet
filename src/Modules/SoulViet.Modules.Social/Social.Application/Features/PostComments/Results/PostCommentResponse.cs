using SoulViet.Modules.Social.Social.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Results
{
    public class PostCommentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? ParentCommentId { get; set; }

        public List<PostCommentResponse> Replies { get; set; } = new();
    }
}
