using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetCommentReplies;
public class GetCommentRepliesQuery : IRequest<Connection<PostCommentResponse>?>
{
    public Guid CommentId { get; set; }
    public string? After { get; set; }
    public int First { get; set; } = 20;
    public string SortBy { get; set; } = "newest";
}
