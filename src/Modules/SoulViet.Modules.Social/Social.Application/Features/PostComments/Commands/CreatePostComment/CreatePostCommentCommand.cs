using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.CreatePostComment
{
    public class CreatePostCommentCommand : IRequest<PostCommentResponse>
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

    }
}
