using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.DeletePostComment
{
    public class DeletePostCommentCommand : IRequest<DeletePostCommentResponse>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
