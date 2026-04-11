using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostCommentById
{
    public class GetPostCommentByPostIdQuery : IRequest<List<PostCommentDto>>
    {
        public Guid PostId { get; set; }
    }
}
