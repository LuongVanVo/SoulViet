using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.DTOs;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Queries.GetPostLikers
{
    public class GetPostLikersQuery : IRequest<Connection<PostLikerDto>?>
    {
        public Guid PostId { get; set; }
        public Guid? CurrentUserId { get; set; }
        public string? After { get; set; }
        public int First { get; set; } = 20;
    }
}
