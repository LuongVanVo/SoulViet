using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostComments;

public class GetPostCommentsQuery : IRequest<Connection<PostCommentResponse>?>
{
    public Guid PostId { get; set; }
    public string? After { get; set; }
    public int First { get; set; } = 20;
    public string SortBy { get; set; } = "newest"; 
    public bool IncludeReplies { get; set; } = false;
}
