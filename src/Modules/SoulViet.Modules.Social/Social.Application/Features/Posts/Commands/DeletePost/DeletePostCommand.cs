using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.DeletePost;

public class DeletePostCommand : IRequest<DeletePostResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

