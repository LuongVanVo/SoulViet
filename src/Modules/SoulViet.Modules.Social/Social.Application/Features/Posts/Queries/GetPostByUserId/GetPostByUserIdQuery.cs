using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostByUserId
{
    public class GetPostByUserIdQuery : IRequest<Connection<PostResponse>?>
    {
        public Guid UserId { get; set; }
        public string? After { get; set; }
        public int First { get; set; } = 20;
        public string SortBy { get; set; } = "newest";
    }
}
