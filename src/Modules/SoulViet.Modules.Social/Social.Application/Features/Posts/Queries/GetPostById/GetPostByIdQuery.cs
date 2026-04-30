using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById;

public class GetPostByIdQuery : IRequest<PostResponse>
{
    public Guid Id { get; set; }        
    public Guid? UserId { get; set; }
}

