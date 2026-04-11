using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Results
{
    public class DeletePostCommentResponse : IRequest<DeletePostCommentResponse>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
